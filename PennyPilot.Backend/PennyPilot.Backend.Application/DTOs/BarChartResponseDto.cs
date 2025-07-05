using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PennyPilot.Backend.Application.DTOs
{
    public class BarChartResponseDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<BarChartDatasetDto> Datasets { get; set; } = new List<BarChartDatasetDto>();
    }

    public class  BarChartDatasetDto
    {
        public string Label { get; set; } = null!;
        public List<decimal> Data { get; set; } = new List<decimal>();
    }
}
