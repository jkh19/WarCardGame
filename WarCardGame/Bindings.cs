using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Interfaces;
using WarCardGame.Services;

namespace WarCardGame
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IDealerService>().To<DealerService>();
            Bind<IGameRoundService>().To<GameRoundService>();
            Bind<IPlayerService>().To<PlayerService>();
            Bind<IWarGameController>().To<WarGameController>();
        }
    }
}
