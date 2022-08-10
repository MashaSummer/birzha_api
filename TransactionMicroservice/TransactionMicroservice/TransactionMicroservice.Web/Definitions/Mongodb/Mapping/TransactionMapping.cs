using AutoMapper;
using Google.Protobuf.Collections;
using OrdersEvent;
using TransactionMicroservice.Definitions.Mongodb.Models;
using Transactions;

namespace TransactionMicroservice.Definitions.Mongodb.Mapping;

public class TransactionMapping : Profile
{
    public TransactionMapping()
    {
        CreateMap<CandidatesFoundEvent, TransactionModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.CreatedTime, s => s.Ignore())
            .ForMember(d => d.AskIds, s => s.MapFrom(x => x.AskIds.ToArray()))
            .ForMember(d => d.BidIds, s => s.MapFrom(x => x.BidIds.ToArray()));

        CreateMap<TransactionModel, TransactionEvent>()
            .ForMember(d => d.AskIds, s => s.MapFrom(x => x.AskIds.ToRepeatedField()))
            .ForMember(d => d.BidIds, s => s.MapFrom(x => x.BidIds.ToRepeatedField()));
    }
}