namespace MoveMate.Repository.Repositories.Dtos;

public class CalculateStatisticTransactionDto
{
    public string Shard { get; set; }
    public double TotalIncome { get; set; }
    public double TotalCompensation  { get; set; }
}