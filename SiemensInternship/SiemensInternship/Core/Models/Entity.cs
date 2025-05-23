﻿namespace LibraryManagement.Core.Models;

public abstract class Entity<TId>
{
    public TId Id { get; set; } = default!;
}