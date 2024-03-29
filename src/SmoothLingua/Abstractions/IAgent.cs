﻿namespace SmoothLingua.Abstractions;

public interface IAgent
{
    Response Handle(string conversationId, string input);

    void Reset(string conversationId);
}
