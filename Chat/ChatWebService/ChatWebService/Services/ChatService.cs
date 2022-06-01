using ChatWebService.Helpers;
using ChatWebService.Models;
using ChatWebService.Models.ChatDb;
using ChatWebService.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatWebService.Services
{
    public class ChatService : IChatService
    {
        #region Fields

        private readonly ChatDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        #endregion

        public ChatService(ChatDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region Public Methods

        public IEnumerable<Message> GetAllMessagesByOperatorId(String operatorId)
        {
            Guid operatorIdGuid = new Guid();
            var isParsed = Guid.TryParse(operatorId, out operatorIdGuid);

            if (!isParsed)
            {
                return null;
            }

            try
            {
                using (_context)
                {
                    return _context.Messages.Where(message => message.OperatorId == operatorIdGuid).Select(message => message);
                }
            }
            catch (ArgumentNullException ex)
            {
                return null;
            }
        }

        public Int32? GetUnreadMessagesCountByOperatorId(String operatorId)
        {
            Guid operatorIdGuid = new Guid();
            var isParsed = Guid.TryParse(operatorId, out operatorIdGuid);

            if (!isParsed)
            {
                return null;
            }

            try
            {
                using (_context)
                {
                    return _context.Messages.Where(message => message.OperatorId == operatorIdGuid)
                        .Where(message => message.IsRead == false)
                        .Count();
                }
            }
            catch (ArgumentNullException ex)
            {
                return null;
            }
        }

        public async Task<Boolean> CreatePlayerConnection(Guid playerId, String connectionId)
        {
            try
            {
                using (_context)
                {
                    var targetPlayer = _context.Players.FirstOrDefault(player => player.Id == playerId);
                    if (targetPlayer == null)
                    {
                        var user = GetIdentityUserById(playerId.ToString());

                        if (user == null)
                        {
                            return false;
                        }

                        targetPlayer = new Player { Id = playerId, Email = user.Email };

                        await _context.Players.AddAsync(targetPlayer);
                        _context.SaveChanges();
                    }

                    var newConnection = new PlayerConnection { ConnectionId = connectionId };
                    targetPlayer.PlayerConnections.Add(newConnection);
                    _context.SaveChanges();
                    return true;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<Boolean> CreateOperatorConnection(Guid operatorId, String connectionId)
        {
            try
            {
                using (_context)
                {
                    var targetOperator = _context.Operators.FirstOrDefault(oper => oper.Id == operatorId);
                    if (targetOperator == null)
                    {
                        var user = GetIdentityUserById(operatorId.ToString());

                        if (user == null)
                        {
                            return false;
                        }

                        targetOperator = new Operator { Id = operatorId, Email = user.Email };

                        await _context.Operators.AddAsync(targetOperator);
                        _context.SaveChanges();
                    }

                    var newConnection = new OperatorConnection { ConnectionId = connectionId };
                    targetOperator = _context.Operators.First(oper => oper.Id == operatorId);
                    targetOperator.OperatorConnections.Add(newConnection);

                    _context.SaveChanges();
                    return true;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<(Boolean, IList<ConnectionBase>)> SendMessage(Sender sender, Guid senderId, Message message)
        {
            try
            {
                switch (sender)
                {
                    case Sender.Player:
                        using (_context)
                        {
                            var player = _context.Players.First(player => player.Id == senderId);
                            player.Messages.Add(message);
                            _context.SaveChanges();

                            var receiverConnections = _context.Operators
                                .Include(oper => oper.OperatorConnections)
                                .First(oper => oper.Id == message.OperatorId)
                                .OperatorConnections.Cast<ConnectionBase>().ToList();

                            return (true, receiverConnections);
                        }

                    case Sender.Operator:
                        using (_context)
                        {
                            var oper = _context.Operators.First(oper => oper.Id == senderId);
                            oper.Messages.Add(message);
                            _context.SaveChanges();

                            var receiverConnections = _context.Players
                                .Include(player => player.PlayerConnections)
                                .First(player => player.Id == message.PlayerId)
                                .PlayerConnections.Cast<ConnectionBase>().ToList();

                            return (true, receiverConnections);
                        }

                    default:
                        throw new Exception();
                }
            }
            catch (Exception e)
            {
                return (false, null);
            }
        }

        public async Task<Boolean> SetMessagesAsRead(Guid readerId, IEnumerable<Int32> messageIds)
        {
            try
            {
                using (_context)
                {
                    var messages = _context.Messages.Where(message => messageIds.Contains(message.Id) && message.IsRead == false);


                    foreach (var message in messages)
                    {
                        if (message.Owner == Owner.Player)
                        {
                            if (message.OperatorId == readerId)
                            {
                                message.IsRead = true;
                            }
                        }
                        else
                        {
                            if (message.PlayerId == readerId)
                            {
                                message.IsRead = true;
                            }
                        }
                    }

                    _context.Messages.UpdateRange(messages);
                    _context.SaveChanges();

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Guid>> GetChats(Sender sender, Guid senderId)
        {
            try
            {
                using (_context)
                {
                    if (sender == Sender.Operator)
                    {
                        return _context.Messages.Where(message => message.OperatorId == senderId)
                            .GroupBy(message => message.PlayerId)
                            .Select(x => x.Key).ToList();
                    }
                    else if (sender == Sender.Player)
                    {
                        return _context.Messages.Where(message => message.PlayerId == senderId)
                          .GroupBy(message => message.OperatorId)
                          .Select(x => x.Key).ToList();
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Message>> GetMessages(Guid operatorId, Guid playerId, Int32 page = -1)
        {
            try
            {
                using (_context)
                {
                    var messages = _context.Messages.
                        Where(message => message.OperatorId == operatorId && message.PlayerId == playerId);

                    if(page == -1)
                    {
                        return messages.Select(x => x).ToList();
                    }
                    else if(page >= 0) 
                    {
                        return messages.Skip(--page * 10).Take(10).ToList();
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private AppUser? GetIdentityUserById(string id) => 
            _userManager.Users.FirstOrDefault(user => user.Id == id);

        #endregion

    }

    public enum Sender
    {
        Player,
        Operator
    }
}