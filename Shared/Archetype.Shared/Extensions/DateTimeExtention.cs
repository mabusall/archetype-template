namespace Archetype.Shared.Extensions;

public static class DateTimeExtension
{
    // -------------------------
    // DateOnly receiver
    // -------------------------
    extension(DateOnly value)
    {
        public DateTime ToDateTime()
            => value.ToDateTime(TimeOnly.MinValue);
    }

    // -------------------------
    // TimeOnly receiver
    // -------------------------
    extension(TimeOnly value)
    {
        public DateTime ToDateTime()
            => DateTime.MinValue + value.ToTimeSpan();
    }

    // -------------------------
    // DateTime receiver
    // -------------------------
    extension(DateTime value)
    {
        public DateOnly ToDateOnly()
            => DateOnly.FromDateTime(value);

        public TimeOnly ToTimeOnly()
            => TimeOnly.FromDateTime(value);
    }

    /// <summary>
    /// Converts a TimeSpan into a Hangfire cron expression.
    ///
    /// - For whole-minute (or larger) intervals, returns 5-field cron:
    ///   "min hour day month day-of-week" (classic Hangfire docs / Cron helper style).
    ///
    /// - If the TimeSpan includes seconds (not a whole minute), returns 6-field cron:
    ///   "sec min hour day month day-of-week" (supported in Hangfire 1.7+).
    ///
    /// Notes:
    /// - Many cron engines (and some Hangfire setups) don't support steps > 59 in
    ///   seconds/minutes fields. This method throws for such cases to avoid invalid schedules.
    /// - Hangfire recurring scheduling runs on a polling model, so second-level precision
    ///   isn't guaranteed even with 6-field expressions.
    /// </summary>
    extension(TimeSpan timeSpan)
    {
        public string ToCronExpression(bool includeSecondsIfNeeded = true)
        {
            // A common "never" cron expression (Feb 31st) to represent non-positive intervals.
            if (timeSpan <= TimeSpan.Zero)
                return $"0 0 31 2 *";

            // Normalize: ignore sub-second resolution for cron purposes
            if (timeSpan.Milliseconds != 0 || timeSpan.Ticks % TimeSpan.TicksPerSecond != 0)
                throw new NotSupportedException("Cron expressions can't represent sub-second intervals. Use whole seconds or more.");

            // If caller allows seconds and the TimeSpan isn't a whole number of minutes,
            // use 6-field cron (sec min hour day month day-of-week).
            // Hangfire 1.7+ accepts 6-part cron strings. [3](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/extension-members)[4](https://github.com/dotnet/csharplang/discussions/164)
            if (includeSecondsIfNeeded && timeSpan.TotalSeconds < 60 && timeSpan.TotalSeconds % 1 == 0)
            {
                int seconds = (int)timeSpan.TotalSeconds;

                // Cron field ranges for seconds are typically 0-59; steps over 59 are invalid in practice.
                if (seconds < 1 || seconds > 59)
                    throw new NotSupportedException("Second-based cron intervals must be between 1 and 59 seconds.");

                return $"*/{seconds} * * * * *";
            }

            // If it contains seconds but is >= 60 seconds, enforce whole minutes.
            // Otherwise a 5-field cron can't represent it.
            if (timeSpan.Seconds != 0)
            {
                // If you want to support "every 90 seconds", you need a different mechanism than cron.
                // Hangfire docs emphasize recurring checks on a minute-based interval. [1](https://www.c-sharpcorner.com/article/hangfire-cron-jobs-full-details-using-asp-net-core/)[5](https://andrewlock.net/exploring-dotnet-10-preview-features-3-csharp-14-extensions-members/)
                throw new NotSupportedException(
                    "This TimeSpan includes seconds but is not a whole minute. " +
                    "Cron can't reliably express mixed minute+second intervals (e.g., 90 seconds). " +
                    "Use a whole number of minutes or use a different scheduling approach.");
            }

            // Whole minutes (5-field cron)
            if (timeSpan.TotalMinutes >= 1 && timeSpan.TotalMinutes % 1 == 0 && timeSpan.TotalHours < 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;

                // Many cron parsers restrict minute steps to <= 59. Protect against invalid */90 etc.
                if (minutes < 1 || minutes > 59)
                    throw new NotSupportedException("Minute-based cron intervals must be between 1 and 59 minutes.");

                return $"*/{minutes} * * * *";
            }

            // Whole hours (5-field cron)
            if (timeSpan.TotalHours >= 1 && timeSpan.TotalHours % 1 == 0 && timeSpan.TotalDays < 1)
            {
                int hours = (int)timeSpan.TotalHours;

                // Hours field supports 0-23; step > 23 isn't broadly valid.
                if (hours < 1 || hours > 23)
                    throw new NotSupportedException("Hour-based cron intervals must be between 1 and 23 hours.");

                return $"0 */{hours} * * *";
            }

            // Whole days (5-field cron)
            if (timeSpan.TotalDays >= 1 && timeSpan.TotalDays % 1 == 0)
            {
                int days = (int)timeSpan.TotalDays;

                // Day-of-month steps are 1-31 in typical cron; */N is allowed but N > 31 makes no sense.
                if (days < 1 || days > 31)
                    throw new NotSupportedException("Day-based cron intervals must be between 1 and 31 days.");

                return $"0 0 */{days} * *";
            }

            // If we got here, it's a fractional hour/day or otherwise not representable by our supported patterns.
            throw new NotSupportedException(
                "TimeSpan can't be converted to a simple cron interval. " +
                "Use whole seconds (<60), whole minutes (<60), whole hours (<24), or whole days (<=31).");
        }
    }
}