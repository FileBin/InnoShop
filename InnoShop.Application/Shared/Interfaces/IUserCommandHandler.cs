namespace InnoShop.Application.Shared.Interfaces;

public interface IUserCommandHandler : ICommandHandler {}

public interface IUserCommandHandler<in TCommand, TResponse> : ICommandHandler<TCommand, TResponse>, IUserCommandHandler
    where TCommand : ICommand<TResponse> { }

public interface IUserCommandHandler<in TCommand> : ICommandHandler<TCommand>, IUserCommandHandler
where TCommand : ICommand { }