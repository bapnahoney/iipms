(function ($) {
    $(document).ready(function () {
       
    });
    $(document).bind("contextmenu", function (e) {
        return false;
    });
    document.onkeydown = (e) => {
        if (e.key == 123) {
            e.preventDefault();
        }
        if (e.ctrlKey && e.shiftKey && e.key == 'I') {
            e.preventDefault();
        }
        if (e.ctrlKey && e.shiftKey && e.key == 'C') {
            e.preventDefault();
        }
        if (e.ctrlKey && e.shiftKey && e.key == 'J') {
            e.preventDefault();
        }
        if (e.ctrlKey && e.key == 'U') {
            e.preventDefault();
        }
    };
   
})(jQuery);

//function GetMessage() {
//    var pOText = $("#POSearch").val();
//    $.post(abp.appPath + "IC/CreateSearch/5001" , function (data) {
//        console.log(data);
//    });
//}
$("#POSearch")
    .focus(function () {
        if (this.value === this.defaultValue) {
            this.value = '';
        }
    })
    .blur(function () {
        if (this.value === '') {
            this.value = this.defaultValue;
        }
    });
var resPOsearch;

$("#btnSearch").on("click", function (event) {
    $('#ICCreateTable.output').empty();
    $('#ICCreateTable > tbody').empty(); 
    var pOText = $("#POSearch").val();
    //pOText =document.getElementsByName('#POSearch').value;
    $.ajax({
        url: abp.appPath + 'IC/POSearch/?ponumber=' + pOText,
        data: 'ponumber=' + pOText,
        type: "Post",
        contentType: "application/json"
    }).done(function (data) {
        if (data && data.result) {
            $("#btnSubmit").removeAttr('disabled');
            if (data.result.poType == 'ZPIS' || data.result.poType == 'ZPDS') {
                alert('This PO type belongs to RFI section.');
                return;
            }
            document.getElementById('vendorName').setAttribute("value", data.result.vendorName);
            document.getElementById('vendorNo').setAttribute("value", data.result.vendorNo == undefined ? "" : data.result.vendorNo);
            document.getElementById('projectName').setAttribute("value", data.result.projectName == undefined ? "" : data.result.projectName);
            //document.getElementById('manufacturerName').setAttribute("value", data.result.manufacturerName == undefined ? "" : data.result.manufacturerName);
            //document.getElementById('manufacturerPlantAddress').setAttribute("value", data.result.manufacturerPlantAddress == undefined ? "" : data.result.manufacturerPlantAddress);
            document.getElementById('vendorRemark').setAttribute("value", data.result.vendorRemark == undefined ? "" : data.result.vendorRemark);

            if (data.result.icItems) {
                resPOsearch = data.result;
                $("#ICCreateTable tbody").find("tr:gt(0)").remove();

                //$("#ICTable").DataTable({
                //    paging: true,
                //    processing: true,
                //    serverSide: true,
                //    responsive: true,
                //    });
                $('#ICCreateTable').find('tbody')
                    .append('<tr><th>ItemNumber</th><th>MaterialNo</th><th>MaterialDescription</th><th>MaterialClass</th><th>UOM</th><th>poQty</th><th>InspectionQty(Previous)</th><th>InspectionQty(Balance)</th><th>Enter Inspection Qty</th><th>Make</th><th>Model</th><th>PartNo</th></tr>');
                $.each(data.result.icItems, function (i,v,row) {


                    $('#ICCreateTable').find('tbody')
                        //.append('<tr><th>Item Number</th><th>MaterialNo</th><th>MaterialDescription</th><th>materialClassValue</th><th>UOM</th><th>poQty</th><th>InspectionQty(Previous)</th><th>InspectionQty(Balance)</th><th>Enter Inspection Qty</th><th>Class/Category</th><th>Status</th><th>Actions</th></tr>')
                        .append('<tr>').append('<td id=' + i +'itemNo>' + v.itemNo + '</td>')
                        .append('<td  id='+i+'materialNo>' + v.materialNo + '</td>')
                        .append('<td  id=' + i + 'materialDescription>' + v.materialDescription + '</td>')
                        .append('<td  id=' + i + 'materialClassValue>' + v.materialClassValue + '</td>')
                        .append('<td  id='+i+'uOM>' + v.uom + '</td>')
                        .append('<td  id='+i+'poQty">' + v.poQty + '</td>')
                        .append('<td  id='+i+'icPreviousQty>' + v.icPreviousQty + '</td>')
                        .append('<td  id='+i+'icBalanceQty>' + v.icBalanceQty + '</td>')
                        //.append('<td> <input type="text" id=' + i + '></td>')
                        .append('<td> <input type="text" id=' + i + 'inputQty value=0></td>')
                        .append('<td> <input type="text" id=' + i + 'make></td>')
                        .append('<td> <input type="text" id=' + i + 'model></td>')
                        .append('<td> <input type="text" id=' + i + 'partNo></td>')
                        //.append('<td  id='+i+'materialClass>' + v.materialClassValue + '</td>')
                        //.append('<td>' + v.statusValue + '</td>')
                        //.append('<td><button type="button" class="btn btn-sm bg-secondary">Details</button></td>')
                        //.append('<td><button width="25%" type="button" class="btn btn-sm bg-secondary edit-ic" data-ic-id="' + ${ row.id } + '" data-toggle="modal" data-target="#ICEditModal"><i class="fas fa-pencil-alt"></i> Edit </button>'
                        //<button type = "button" class= "btn btn-sm bg-danger delete-role" data - role - id="1" data - role - name="Admin" > <i class="fas fa-trash"></i> Delete</button ></td > ')
                        //.append('<td width="25%" class="Discount">' + v.Discount + '</td>')
                        //.append('<td width="25%" class="TotalPrice">' + v.TotalPrice + '</td>')
                        .append('<tr>');
                });
            }
        }
      
    }).fail(function (e, jqXHR, textStatus, errorThrown) {

        if (e.responseJSON.error.code === 203) {
            alert("User is not mapped with this PO.");
        }
        else if (e.responseJSON.error.code == 204) {
            alert("PO detail not available in SAP.");
        }
        else if (e.responseJSON.error.code == 403) {
            alert("Unable to fetch PO detail.");
        }
        else {
            alert(jqXHR.responseJSON.error.message);
        }
    });
});

$("#btnSubmit").on("click", function (event) {
   // console.log(resPOsearch);
    //resPOsearch.manufacturerName = document.getElementById('manufacturerName').value;
//resPOsearch.manufacturerPlantAddress = document.getElementById('manufacturerPlantAddress').value;
  //  resPOsearch.materialNo = document.getElementById('materialNo').value;
    resPOsearch.vendorRemark = document.getElementById('vendorRemark').value;
    
    jQuery.each(resPOsearch.icItems, function (i, val) {
        var inputVal = document.getElementById(i + 'inputQty').value;
        var iNum = parseInt(inputVal);
        var balVal = document.getElementById(i + 'icBalanceQty').value;
        var iNum = parseInt(balVal);
        if (inputVal > balVal) {
            console.log("Error");
            return;
        }
        resPOsearch.icItems[i].make = document.getElementById(i + 'make').value;
        resPOsearch.icItems[i].model = document.getElementById(i + 'model').value;
        resPOsearch.icItems[i].partNo = document.getElementById(i + 'partNo').value;
        resPOsearch.icItems[i].icInputQty = inputVal;
        //resPOsearch.icItems[i].icBalanceQty = balVal;
        
    });
    callCreateICRequest();
});

$("#btnEdit").on("click", function (event) {
    console.log(resPOsearch);
  
});


function callCreateICRequest() {

    $.ajax({
        type: 'POST', //HTTP POST Method
        url: abp.appPath + 'IC/ICRequest/',// Controller/View 
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(resPOsearch),
        success: function (content, data) {
            if (data == 'success') {
               
                window.location =  window.location.href.replace("/Create", '/');
                //Response.redirect(abp.appPath + "/IC");
            }
            else {
                abp.ajax.showError(content.result);
            }
        },
        error: function (e, content) {
            alert(e.responseJSON.error.message)
        }
    });
}

function setDataOnRow(rowObject, v) {

    // debugger
    $(rowObject).find(".itemNo").html(v.itemNo);
    $(rowObject).find(".materialNo").html(v.materialNo);
    $(rowObject).find(".materialDescription").html(v.materialDescription);
    $(rowObject).find(".poQty").html(v.poQty);
    $(rowObject).find(".status").html(v.status);
    
}

function getPODetail() {
    var pOText = $("#POSearch").val();
    $('#ICCreateTable thead th').each(function () {
        debugger;
        if ($(this).index() == 1) {
            var title = $('#ICCreateTable thead th').eq($(this).index()).text();
            $(this).html('<form onsubmit="return false" autocomplete="off"><input type="text" placeholder="' + title + '" /></form>');
        }
    });
    var table = $('#ICCreateTable').DataTable({
        "processing": true,
        "serverSide": true,
        "responsive": true,
        "dom": 'rt<"bottom"flp><"clear">',
        //"lengthMenu": [10, 25, 50],
        "ajax": {
            //url: abp.appPath + 'PO/PODetail/' + pOText,
            url: abp.appPath + 'IC/POSearch/' + pOText, 
            dataType: 'html',
            //"url": '@Url.Action("getCityList", "Masters")',
            //"type": "POST",
            data: {

                columnsDef: ['PONo', 'VendorName', 'VendorNo', 'ProjectName'],
            },
        },
        "columns": [
            {
                //"targets": 0,
                "title": "Actions",
                // "orderable": false, 
                //"sClass": "GridAction",
                "render": function (data, type, full, meta) {
                    console.log('on close event fired!');
                    return '<a href="@Url.Action("City", "Masters")?id=' + full.id + '" class="btn btn-sm btn-clean btn-icon btn-icon-md"><i class="fa fa-tasks"></i></a>';
                }
            },
            { "data": "PO", "orderable": false },
            { "data": "vendorName", "orderable": false },
            { "data": "vendorNo", "orderable": false },
            { "data": "projectName", "orderable": false },

        ],
        "columnDefs": [
            { "orderable": false, "targets": 0 }
        ],
        "order": [],
    });

    // Apply the filter
    table.columns().every(function () {
        var column = this;
        $('input', this.header()).on('keyup change', function () {
            column.search(this.value).draw();
        });
    });
};
