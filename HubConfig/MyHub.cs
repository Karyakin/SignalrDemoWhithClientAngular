using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalrDemo.EFModels;
using SignalrDemo.HubModels;

namespace SignalrDemo.HubConfig
{
    public partial class MyHub : Hub
    {

        private readonly IDbContext _dbContext;
        public MyHub(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var currUserId = _dbContext.Connections
                .Where(c => c.SignalrId == Context.ConnectionId)
                .Select(c => c.personId)
                .SingleOrDefault();

            _dbContext.Connections.RemoveRange(_dbContext.Connections.Where(p => p.personId == currUserId).ToList());
            _dbContext.SaveChanges();

            Clients.Others.SendAsync("userOff", currUserId);
            return base.OnDisconnectedAsync(exception);
        }


        #region First Part
        public async Task AskServer(string someTextFromClient)
        {
            string tempString;
            tempString = someTextFromClient == "hey" ? "message was 'hey'" : "message was something else";

            // у всех подключенных клиентов вызовется функция askServerResponse(соответсвенно, название функции должно быть таким же) и передадим в нее значение tempstring
            //Client - по сути подключение, два браузера == два подключения(клиента)
            await Clients.Client(this.Context.ConnectionId).SendAsync("askServerResponse", tempString);
        }
        #endregion




        public async Task AuthMe(PersonInfo personInfo)
        {
            var currSignalrID = Context.ConnectionId;// получаем айдишник текущего соединения

            var tempPerson = await _dbContext.Persons.FirstOrDefaultAsync(x =>
                x.Username == personInfo.UserName && x.Password == personInfo.Password);// получаем зареганного ранее пользователя из базы


            if (tempPerson != null)
            {
                Console.WriteLine("\n" + tempPerson.personName + " logged in" + "\nSignalrID: " + currSignalrID);

                var currentConnection = new Connections//создаем соединение для записи его в базу данных
                {
                    Id = Guid.NewGuid(),
                    personId = tempPerson.Id,
                    Person = tempPerson,
                    SignalrId = currSignalrID,
                    TimeStamp = DateTime.Now
                };

                await _dbContext.Connections.AddAsync(currentConnection);
                await _dbContext.SaveChangesAsync();

                var newUser = new User(tempPerson.Id, tempPerson.personName, currSignalrID);

                // если я правильно полнял, то сдесь мы просто уведомляем ползователя, который регается прошел ли он авторизацию или нет
                await Clients.Caller.SendAsync("authMeResponseSuccess", newUser);
                await Clients.Others.SendAsync("userOn", newUser);//4Tutorial

            }
            else
            {
                await Clients.Caller.SendAsync("authMeResponseFail");
            }
        }

        public async Task ReauthMe(Guid personId)
        {
            var currSignalrID = Context.ConnectionId;

            var tempPerson = await _dbContext.Persons.Where(p => p.Id == personId).FirstOrDefaultAsync();
            if (tempPerson != null)
            {
                Console.WriteLine("\n" + tempPerson.personName + " logged in" + "\nSignalrID: " + currSignalrID);

                var currentConnection = new Connections
                {
                    personId = tempPerson.Id,
                    SignalrId = currSignalrID,
                    TimeStamp = DateTime.Now
                };
                await _dbContext.Connections.AddAsync(currentConnection);
                await _dbContext.SaveChangesAsync();
            }

            var newUser = new User(tempPerson.Id, tempPerson.personName, currSignalrID);

            // если я правильно полнял, то сдесь мы просто уведомляем ползователя, который регается прошел ли он авторизацию или нет
            await Clients.Caller.SendAsync("authMeResponseSuccess", newUser);
            await Clients.Others.SendAsync("userOn", newUser);//4Tutorial
        }

        public void LogOut(Guid personId)
        {
            _dbContext.Connections.RemoveRange(_dbContext.Connections.Where(p => p.personId == personId).ToList());
            _dbContext.SaveChanges();
            Clients.Caller.SendAsync("logoutResponse");
            Clients.Others.SendAsync("userOff", personId);
        }
    }
}
