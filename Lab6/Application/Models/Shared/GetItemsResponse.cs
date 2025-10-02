namespace Lab6.Application.Models.Shared;

public record GetItemsResponse<T>(int Count, T[] Items);
