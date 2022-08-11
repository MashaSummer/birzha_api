using AutoMapper;
using OrdersEvent;
using TransactionMicroservice.Definitions.Mongodb.Models;

namespace TransactionMicroservice.Definitions.Mongodb.Mapping;

public class TransactionMapping : Profile
{
    public TransactionMapping()
    {
        CreateMap<CandidatesFoundEvent, TransactionModel>()
            .ForMember(d => d.Id, s => s.Ignore())
            .ForMember(d => d.CreatedTime, s => s.Ignore())
            .ForMember(d => d.AskId, s => s.MapFrom(x => x.AskCandidate.OrderId))
            .ForMember(d => d.BidId, s => s.MapFrom(x => x.BidCandidate.OrderId));
    }
}