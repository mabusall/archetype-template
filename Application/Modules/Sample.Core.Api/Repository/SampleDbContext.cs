namespace Sample.Core.Api.Repository;

class SampleDbContext(DbContextOptions<SampleDbContext> options,
                      IIdGeneratorService idGeneratorService,
                      ICurrentUserService currentUserService) :
    GenericDbContext(options, idGeneratorService, currentUserService)
{
    protected override string Schema => "dbo";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        IEnumerable<IMutableForeignKey> foriegnKeys = modelBuilder
            .Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());

        foreach (IMutableForeignKey? relationship in foriegnKeys)
            relationship.DeleteBehavior = DeleteBehavior.Cascade;

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
}