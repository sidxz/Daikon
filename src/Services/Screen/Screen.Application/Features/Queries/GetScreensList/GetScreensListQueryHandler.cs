using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Screen.Application.Contracts.Persistence;
using Screen.Application.Features.Queries.ViewModels;

namespace Screen.Application.Features.Queries.GetScreensList
{
    public class GetScreensListQueryHandler : IRequestHandler<GetScreensListQuery, List<ScreensListVM>>
    {
        private readonly IMediator _mediator;
        private readonly IScreenRepository _screenRepository;
        private readonly ILogger<GetScreensListQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetScreensListQueryHandler(IMediator mediator, IScreenRepository screenRepository, ILogger<GetScreensListQueryHandler> logger, IMapper mapper)
        {
            _mediator = mediator;
            _screenRepository = screenRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<ScreensListVM>> Handle(GetScreensListQuery request, CancellationToken cancellationToken)
        {
            var screens = await _screenRepository.GetScreensList();
            var screensListVm = _mapper.Map<List<ScreensListVM>>(screens, opts => opts.Items["WithMeta"] = request.WithMeta);

            return screensListVm;
        }
    }
}