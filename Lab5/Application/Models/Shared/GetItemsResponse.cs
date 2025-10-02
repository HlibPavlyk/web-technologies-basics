namespace Lab5.Application.Models.Shared;

public record GetItemsResponse<T>(int Count, T[] Items);
