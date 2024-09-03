(function ($) {


    //$(document).ready(function () {



    //    function callDownloadFile(parameter) {
    //        // Use AJAX to load content into the modal
    //        $.ajax({
    //            url: '@Url.Action("GetMemoryStreamData", "File")',
    //            type: 'GET',
    //            data: { fileName: parameter },
    //            success: function (data) {
    //                //$('#memoryStreamModal .modal-body').html(data);
    //                $('#memoryStreamModal .modal-body').load(data);
    //                $('#memoryStreamModal').modal('show');
    //                var modal = $(this);
    //                modal.find('.modal-dialog').css({
    //                    'min-width': '80%',
    //                    'top': '0%', // Adjust the top position
    //                    'left': '10%', // Adjust the left position
    //                    'margin-left': function () {
    //                        return -($(this).width() / 2);
    //                    }
    //                });

    //            }
    //        });
    //    }


    //})
    //$(document).bind("contextmenu", function (e) {
    //    return false;
    //});
    //document.onkeydown = (e) => {
    //    if (e.key == 123) {
    //        e.preventDefault();
    //    }
    //    if (e.ctrlKey && e.shiftKey && e.key == 'I') {
    //        e.preventDefault();
    //    }
    //    if (e.ctrlKey && e.shiftKey && e.key == 'C') {
    //        e.preventDefault();
    //    }
    //    if (e.ctrlKey && e.shiftKey && e.key == 'J') {
    //        e.preventDefault();
    //    }
    //    if (e.ctrlKey && e.key == 'U') {
    //        e.preventDefault();
    //    }
    //};
   
 
    $('[id^="fileInput"]').change(function (evt) {
        var fileData = new FormData();
        fileData.append("Id", evt.target.id);
        fileData.append("Module", "RFI");
        //fileData.append("RequestId", "1");
        var files1 = $('[id*=' + evt.target.id + ']').get(0).files;
        //var files1 = $(this.event.target.id).get(0).files;
        for (var i = 0; i < files1.length; i++) {
            fileData.append("fileInput", files1[i]);
        }
        if (fileData) {
            $.ajax({
                type: "POST",
                //url: "/File/UploadFiles",
                url: "/File/UploadFiles/",
                dataType: "json",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (result, status, xhr, data) {
                    console.log(result.result);

                    if (result.result == 'Error!') {
                        $("#RFIDocumentTable tbody").find("tr:gt(0)").remove();
                    }
                    else {
                        var docObj = result.result;
                        $("#RFIDocumentTable tbody").find("tr:gt(0)").remove();

                        //$('#RFIDocumentTable').find('tbody')
                        //    .append('<tr><th>Id</th><th>DocumentPath</th></tr>');
                        $.each(docObj, function (i, v, row) {

                            $.each(v.itemDocuments, function (j, w) {
                            
                                onclick = "window.open(this.src, '_blank');"

                                $("#RFIDocumentTable").last()
                                    .append($('<tr>').append('<td id=' + i + 'id>' + v.requestedItemId + '</td>')
                                        .append($('<td>')
                                            .append($('<img>')
                                                //.attr('src', 'https://simple.wikipedia.org/wiki/Link#/media/File:Chain_link_icon.png')
                                                .attr('src', w.documentLocation)
                                                .attr('onclick', "window.open(this.src, '_blank')")
                                                .text('Image cell')
                                            )
                                        )
                                    );
                            });
                        });
                        $("#nthRow").remove();
                    }
//                    else {
//                        window.location.reload();
//}
                    //else {
                    //    var docObj = result.result;
                    //    $("#RFIDocumentTable tbody").find("tr:gt(0)").remove();

                    //    //$('#DocumentTable').find('tbody')
                    //    //    .append('<tr><th>Id</th><th>DocumentPath</th></tr>');
                    //    $.each(docObj, function (i, v, row) {

                    //        var $row = $('<tr>' +
                    //            '<td id=' + i + 'id>' + v.requestedItemId + '</td>'
                    //            );
                            
                    //        $.each(v.itemDocuments, function (j, w) {
                    //            $row += $('<td><label id=' + j + 'id>' + w.documentLocation + '</label></td>');
                                
                    //        });
                    //         $row += $('</tr>');
                    //        $('#RFIDocumentTable > tbody:last-child').append($row);
                    //    });

                    //}

                },
                error: function (xhr, status, error) {
                     alert(error);
                }
            });
        }
        //alert($(this).val());
    });

})(jQuery);