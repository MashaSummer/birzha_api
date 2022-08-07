using AutoMapper;
using TransactionMicroservice.Definitions.Mongodb.Models;
using Transactions;

namespace TransactionMicroservice.Definitions.Mongodb.Mapping;

public class TransactionMapping : Profile
{
    public TransactionMapping()
    {
        CreateMap<Transaction, TransactionModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.CreatedTime, s => s.Ignore());
    }
}