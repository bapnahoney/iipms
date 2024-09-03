using System.Collections.Generic;

namespace HIPMS.IC.Dto;

public class SearchPORequest
{
    public string PONumber { get; set; }
}
public class POSAPResponse
{
    public POSAPResponse()
    {
        PO_DETAILS = new PO_DETAILS();
    }
    public string VENDOR_NAME { get; set; }
    public string VENDOR_NO { get; set; }
    public string PO_TYPE { get; set; }
    public string PROJECT_NAME { get; set; }
    public string CREATED_BY { get; set; }
    public PO_DETAILS PO_DETAILS { get; set; }

}
public class PO_DETAILS
{
    public PO_DETAILS()
    {
        item = new List<Item>();
    }
    public List<Item> item { get; set; }
}

public class Item
{
    public string ITEM_NO { get; set; }
    public string MATERIAL_NO { get; set; }
    public string MATERIAL_DESC { get; set; }
    public string UOM { get; set; }
    public decimal? PO_QUAN { get; set; }
    public string MATERIAL_CLASS { get; set; }
    public string SERVICE_NO { get; set; }
    public string SERVICE_DESC { get; set; }
    public float? SERVICE_QUAN { get; set; }
    public string SERVICE_UOM { get; set; }

}


public class SingleItemPOSAPResponse
{
    public SingleItemPOSAPResponse()
    {
        PO_DETAILS = new SPO_DETAILS();
    }
    public string VENDOR_NAME { get; set; }
    public string VENDOR_NO { get; set; }
    public string PO_TYPE { get; set; }
    public string PROJECT_NAME { get; set; }
    public string CREATED_BY { get; set; }
    public SPO_DETAILS PO_DETAILS { get; set; }

}
public class SPO_DETAILS
{
    public SPO_DETAILS()
    {
        item = new SItem();
    }
    public SItem item { get; set; }
}

public class SItem
{
    public string ITEM_NO { get; set; }
    public string MATERIAL_NO { get; set; }
    public string MATERIAL_DESC { get; set; }
    public string UOM { get; set; }
    public decimal? PO_QUAN { get; set; }
    public string MATERIAL_CLASS { get; set; }
    public string SERVICE_NO { get; set; }
    public string SERVICE_DESC { get; set; }
    public float? SERVICE_QUAN { get; set; }
    public string SERVICE_UOM { get; set; }

}