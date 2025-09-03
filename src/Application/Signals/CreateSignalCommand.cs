using MediatR;
using ProyectoForex.Application.Dtos;

namespace ProyectoForex.Application.Signals;
public sealed record CreateSignalCommand(AlertMessageDto Dto) : IRequest<Guid>;
