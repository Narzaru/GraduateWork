using System;
using System.Collections.Generic;
using System.Linq;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public abstract class Program
{
    public static void Main()
    {
        using var context = new FixedPointsContext();
        context.Database.EnsureCreated();

        // var points = new List<Point>
        // {
        //     new()
        //     {
        //         Id = 1,
        //         PositionX = 0.0f,
        //         PositionY = 0.0f
        //     },
        //     new()
        //     {
        //         Id = 2,
        //         PositionX = 0.0f,
        //         PositionY = 0.0f
        //     },
        //     new()
        //     {
        //         Id = 3,
        //         PositionX = 0.0f,
        //         PositionY = 0.0f
        //     }
        // };
        //
        // var set = new PointsSet()
        // {
        //     Id = 1,
        //     Label = "Aboba",
        //     Points = new List<Point>(points),
        //     CreationTime = DateTime.Now
        // };
        //
        // context.PointsSet!.Add(set);
        // context.Points!.AddRange(points);
        // context.SaveChanges();

        var res = context.PointsSet
            .Include(p => p.Points);

        res.ToList().ForEach(i =>
            i.Points.ToList().ForEach(p => Console.WriteLine($"id={p.Id} x={p.PositionX} y={p.PositionX}")));
    }
}