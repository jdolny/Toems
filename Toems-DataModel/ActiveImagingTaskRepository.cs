using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toems_Common.Entity;

namespace Toems_DataModel
{
    public class ActiveImagingTaskRepository : GenericRepository<EntityActiveImagingTask>
    {
        private readonly ToemsDbContext _context;

        public ActiveImagingTaskRepository(ToemsDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<TaskWithComputer> GetAllTaskWithComputers(int userId)
        {
            return (from h in _context.ActiveImagingTasks
                    join t in _context.Computers on h.ComputerId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where h.UserId == userId
                    select new
                    {
                        id = h.Id,
                        computerId = h.ComputerId,
                        status = h.Status,
                        queue = h.QueuePosition,
                        elapsed = h.Elapsed,
                        remaing = h.Remaining,
                        completed = h.Completed,
                        rate = h.Rate,
                        partition = h.Partition,
                        arguments = h.Arguments,
                        type = h.Type,
                        multicastId = h.MulticastId,
                        userId = h.UserId,
                        comServerId = h.ComServerId,
                        computer = p
                    }).AsEnumerable().Select(x => new TaskWithComputer
                    {
                        Id = x.id,
                        ComputerId = x.computerId,
                        Status = x.status,
                        QueuePosition = x.queue,
                        Elapsed = x.elapsed,
                        Remaining = x.remaing,
                        Completed = x.completed,
                        Rate = x.rate,
                        Partition = x.partition,
                        Arguments = x.arguments,
                        Type = x.type,
                        MulticastId = x.multicastId,
                        UserId = x.userId,
                        ComServerId = x.comServerId,
                        Computer = x.computer ?? new EntityComputer() { Name = x.arguments }
                    }).OrderBy(x => x.Computer.Name).ToList();
        }

        public List<TaskWithComputer> GetAllTaskWithComputersForAdmin()
        {
            return (from h in _context.ActiveImagingTasks
                    join t in _context.Computers on h.ComputerId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()

                    select new
                    {
                        id = h.Id,
                        computerId = h.ComputerId,
                        status = h.Status,
                        queue = h.QueuePosition,
                        elapsed = h.Elapsed,
                        remaing = h.Remaining,
                        completed = h.Completed,
                        rate = h.Rate,
                        partition = h.Partition,
                        arguments = h.Arguments,
                        type = h.Type,
                        multicastId = h.MulticastId,
                        userId = h.UserId,
                        comServerId = h.ComServerId,
                        computer = p
                    }).AsEnumerable().Select(x => new TaskWithComputer
                    {
                        Id = x.id,
                        ComputerId = x.computerId,
                        Status = x.status,
                        QueuePosition = x.queue,
                        Elapsed = x.elapsed,
                        Remaining = x.remaing,
                        Completed = x.completed,
                        Rate = x.rate,
                        Partition = x.partition,
                        Arguments = x.arguments,
                        Type = x.type,
                        MulticastId = x.multicastId,
                        UserId = x.userId,
                        ComServerId = x.comServerId,
                        Computer = x.computer ?? new EntityComputer() { Name = x.arguments }
                    }).OrderBy(x => x.Computer.Name).ToList();
        }

        public List<TaskWithComputer> GetMulticastMembers(int multicastId)
        {
            return (from h in _context.ActiveImagingTasks
                    join t in _context.Computers on h.ComputerId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where h.MulticastId == multicastId && h.Type == "multicast"
                    select new
                    {
                        id = h.Id,
                        computerId = h.ComputerId,
                        status = h.Status,
                        queue = h.QueuePosition,
                        elapsed = h.Elapsed,
                        remaing = h.Remaining,
                        completed = h.Completed,
                        rate = h.Rate,
                        partition = h.Partition,
                        arguments = h.Arguments,
                        type = h.Type,
                        multicastId = h.MulticastId,
                        userId = h.UserId,
                        comServerId = h.ComServerId,
                        computer = p
                    }).AsEnumerable().Select(x => new TaskWithComputer
                    {
                        Id = x.id,
                        ComputerId = x.computerId,
                        Status = x.status,
                        QueuePosition = x.queue,
                        Elapsed = x.elapsed,
                        Remaining = x.remaing,
                        Completed = x.completed,
                        Rate = x.rate,
                        Partition = x.partition,
                        Arguments = x.arguments,
                        Type = x.type,
                        MulticastId = x.multicastId,
                        UserId = x.userId,
                        ComServerId = x.comServerId,
                        Computer = x.computer
                    }).OrderBy(x => x.Computer.Name).ToList();
        }

      

        public List<TaskWithComputer> GetUnicastsWithComputers(int userId)
        {
            return (from h in _context.ActiveImagingTasks
                    join t in _context.Computers on h.ComputerId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where (h.Type == "upload" || h.Type == "deploy") && h.UserId == userId && h.ComputerId > -1
                    select new
                    {
                        id = h.Id,
                        computerId = h.ComputerId,
                        status = h.Status,
                        queue = h.QueuePosition,
                        elapsed = h.Elapsed,
                        remaing = h.Remaining,
                        completed = h.Completed,
                        rate = h.Rate,
                        partition = h.Partition,
                        arguments = h.Arguments,
                        type = h.Type,
                        multicastId = h.MulticastId,
                        userId = h.UserId,
                        comServerId = h.ComServerId,
                        computer = p
                    }).AsEnumerable().Select(x => new TaskWithComputer
                    {
                        Id = x.id,
                        ComputerId = x.computerId,
                        Status = x.status,
                        QueuePosition = x.queue,
                        Elapsed = x.elapsed,
                        Remaining = x.remaing,
                        Completed = x.completed,
                        Rate = x.rate,
                        Partition = x.partition,
                        Arguments = x.arguments,
                        Type = x.type,
                        MulticastId = x.multicastId,
                        UserId = x.userId,
                        ComServerId = x.comServerId,
                        Computer = x.computer
                    }).OrderBy(x => x.Computer.Name).ToList();
        }

        public List<TaskWithComputer> GetUnicastsWithComputersForAdmin()
        {
            return (from h in _context.ActiveImagingTasks
                    join t in _context.Computers on h.ComputerId equals t.Id into joined
                    from p in joined.DefaultIfEmpty()
                    where (h.Type == "upload" || h.Type == "deploy") && h.ComputerId > -1
                    select new
                    {
                        id = h.Id,
                        computerId = h.ComputerId,
                        status = h.Status,
                        queue = h.QueuePosition,
                        elapsed = h.Elapsed,
                        remaing = h.Remaining,
                        completed = h.Completed,
                        rate = h.Rate,
                        partition = h.Partition,
                        arguments = h.Arguments,
                        type = h.Type,
                        multicastId = h.MulticastId,
                        userId = h.UserId,
                        comServerId = h.ComServerId,
                        computer = p
                    }).AsEnumerable().Select(x => new TaskWithComputer
                    {
                        Id = x.id,
                        ComputerId = x.computerId,
                        Status = x.status,
                        QueuePosition = x.queue,
                        Elapsed = x.elapsed,
                        Remaining = x.remaing,
                        Completed = x.completed,
                        Rate = x.rate,
                        Partition = x.partition,
                        Arguments = x.arguments,
                        Type = x.type,
                        MulticastId = x.multicastId,
                        UserId = x.userId,
                        ComServerId = x.comServerId,
                        Computer = x.computer
                    }).OrderBy(x => x.Computer.Name).ToList();
        }

        public List<EntityComputer> MulticastComputers(int multicastId)
        {
            return
                (from t in _context.ActiveImagingTasks
                 join c in _context.Computers on t.ComputerId equals c.Id
                 where t.MulticastId == multicastId
                 orderby t.ComputerId
                 select c)
                    .ToList();
        }

        public List<EntityActiveImagingTask> MulticastProgress(int multicastId)
        {
            return
                (from t in _context.ActiveImagingTasks
                 where t.MulticastId == multicastId && t.Status == Toems_Common.Enum.EnumTaskStatus.ImagingStatus.Imaging
                 orderby t.ComputerId
                 select t).Take(1).ToList();
        }
    }
}
