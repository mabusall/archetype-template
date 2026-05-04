namespace Archetype.Shared.Interfaces;

public interface ITokenRing
{
    string Current { get; }

    string PeekNext();

    void Advance();             // move to next token (on auth failure)

    void SetIndex(int index);   // optional: seed/restore index
}
