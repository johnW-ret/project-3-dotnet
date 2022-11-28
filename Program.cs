using System.Xml.Linq;

List<Job> Jobs = new();

// input will be handled differently in Java or C++, no need to bother
string input = @"A	0	3
B	2	6
C	4	4
D	6	5
E	8	2
";

StringReader sr = new(input);
StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

while (sr.ReadLine() is string line)
{
    string[] tokens = line.Split('	');
    //tokens.ToList().ForEach(t => Console.WriteLine("t" + t + "t"));
    //Console.WriteLine();

    if (tokens is [string name, string arrivalTimeToken, string durationToken]
        && int.TryParse(tokens[1], out int arrivalTime)
        && int.TryParse(tokens[2], out int duration))
    {
        Jobs.Add(new(tokens[0], arrivalTime, duration));
    }
}

Jobs.ForEach(j => Console.WriteLine($"{j.Name}, {j.arrivalTime}, {j.Duration}"));

var scheduler = new Scheduler(1);

List<string> Timeline = scheduler.DeriveSchedule(Jobs);

Timeline.ForEach(Console.WriteLine);

record Job(string Name, int arrivalTime, int Duration);

class Quantum
{
    private readonly uint q;
    private uint time;

    public Quantum(uint q)
    {
        time = this.q = q;
    }

    public bool Tick()
    {
        bool timeout = --time == 0;

        if (timeout)
            time = q;

        return timeout;
    }
}

class Scheduler
{
    class Process
    {
        private readonly Job job;
        private uint remaining;

        public Process(Job job)
        {
            this.job = job;
            remaining = (uint)job.Duration;
        }

        public Job Job => job;

        public bool Run() => --remaining == 0;
    }

    public List<string> DeriveSchedule(List<Job> jobs)
    {
        List<string> Timeline = new()
        {
            string.Join(' ', jobs.Select(j => j.Name))
        };

        jobs.OrderBy(j => j.arrivalTime);

        jobQueue.Enqueue(new(jobs.First()));

        while (jobQueue.Count > 0)
        {
            var runJob = jobQueue.Peek();
            bool finished = runJob.Run();

            // if a timeout
            if (quantum.Tick())
            {
                jobQueue.Dequeue();
            }

            // print run
            List<string> tLine = new List<string> { " ", " ", " ", " ", " " };
            tLine[jobs.IndexOf(runJob.Job)] = runJob.Job.Name;
            Timeline.Add(string.Join(' ', tLine));

            t++;
            if (jobIndex < jobs.Count
                && jobs[jobIndex].arrivalTime == t)
            {
                jobQueue.Enqueue(new(jobs[jobIndex]));
                jobIndex++;
            }

            if (!finished)
                jobQueue.Enqueue(runJob);
        }

        return Timeline;
    }

    private readonly Queue<Process> jobQueue = new();
    private readonly Quantum quantum;
    private int t = 0;
    private int jobIndex = 1;

    public Scheduler(uint quantum)
    {
        this.quantum = new(quantum);
    }
}