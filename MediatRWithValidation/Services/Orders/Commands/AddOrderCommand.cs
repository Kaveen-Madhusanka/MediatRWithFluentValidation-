using AutoMapper;
using FluentValidation;
using MediatR;
using MediatRWithValidation.Mappings;
using MediatRWithValidation.Models;
using MediatRWithValidation.Persistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRWithValidation.Services.Orders.Commands
{
    public class AddOrderCommand : IMapTo<Order>, IRequest<Order>
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string Contact { get; set; }
    }

    public class AddOrderCommandValidator : AbstractValidator<AddOrderCommand>
    {
        public AddOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name should not be empty")
                .MaximumLength(200).WithMessage("Cus");

        }
    }

    public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, Order>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public AddOrderCommandHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public IMapper Mapper { get; }

        public async Task<Order> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
            throw new ArgumentException(nameof(request), "Error msg");
            // mapping

            var order = _mapper.Map<Order>(request);

            await _appDbContext.Order.AddAsync(order);
            await _appDbContext.SaveChangesAsync();

            return order;
        }
    }



}
