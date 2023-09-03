using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Components;

public class SelectComponentModel
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
    public bool will_use_group { get;set;} = false;
    public string value { get; set; } = string.Empty; 
    public List<SelectGroupModel> Group_items { get; set; } = new List<SelectGroupModel>();
    public List<SelectItemComponentModel> Items { get; set;} = new List<SelectItemComponentModel>();
}

public class SelectGroupModel
{
    public string label { get; set;} = string.Empty;
    public List<SelectItemComponentModel> Items { get; set; } = new List<SelectItemComponentModel>();
}

public class SelectItemComponentModel
{
    public string attribute { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
    public string text { get; set; } = string.Empty;
}

