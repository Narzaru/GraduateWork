using System;
using System.Collections.Generic;

namespace Data.Models;

public class PointsSet
{
    public int Id { get; set; }
    public DateTime CreationTime { get; set; }
    public ICollection<Point> Points { get; set; }
    public string Label { get; set; }
}