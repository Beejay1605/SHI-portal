using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Components;

public class InputComponentModel
{
    public string id { get; set; } = string.Empty;
    public string group { get; set; } = string.Empty;
    public string name {get;set;} = string.Empty;
    public string placeholder {get;set;} = string.Empty;
    public string type {get;set;} = "text";
    public string container_style {get;set;} = string.Empty;
    public string input_style {get;set;} = string.Empty;
    public string container_class {get;set;} = string.Empty;
    public string input_class {get;set;} = string.Empty; 
    public string attributes { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
}
