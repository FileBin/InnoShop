namespace InnoShop.Application.Shared.Interfaces;

public interface IProductCommandHandler : ICommandHandler {}

public interface IProductCommandHandler<in TCommand, TResponse> : ICommandHandler<TCommand, TResponse>, IProductCommandHandler
    where TCommand : ICommand<TResponse> { }

public interface IProductCommandHandler<in TCommand> : ICommandHandler<TCommand>, IProductCommandHandler
where TCommand : ICommand { }