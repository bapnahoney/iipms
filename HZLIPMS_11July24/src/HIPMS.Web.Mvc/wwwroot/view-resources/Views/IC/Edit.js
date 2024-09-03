(function ($) {
    $(document).ready(function () {
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

        $('#editform').on('submit', function (e) {
            //alert(e.target.id);
            //abp.message.confirm('AreYouSureWantToApprove',           
            //    null,
            //    (isConfirmed) => {
            //        if (isConfirmed) {
            //            alert("hfjehfj");
            //        }
            //    }
            //);

            // Checking condition
            if (someError) {
                // some error happen, don't submit the form
                e.preventDefault();

                alert(e.responseJSON.error.message);
                // then if you want to submit the form inside this block, use this
                // $("delete-form").submit();
            }
            // other things...
        });
        // ... Other things

    });

    $('[id^="fileInput"]').change(function (evt) {
        var fileData = new FormData();
        fileData.append("Id", evt.target.id);
        fileData.append("Module", "IC");
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
                        $("#DocumentTable tbody").find("tr:gt(0)").remove();
                    }
                    else {
                        var docObj = result.result;
                        $("#DocumentTable tbody").find("tr:gt(0)").remove();

                        //$('#DocumentTable').find('tbody')
                        //    .append('<tr><th>Id</th><th>DocumentPath</th></tr>');
                        $.each(docObj, function (i, v, row) {

                            $.each(v.itemDocuments, function (j, w) {

                                onclick = "window.open(this.src, '_blank');"

                                $("#DocumentTable").last()
                                    .append($('<tr>').append('<td id=' + i + 'id>' + v.requestedItemId + '</td>')
                                        .append($('<td>')
                                            .append($('<a asp-controller="File" asp-action="download" asp-route-fileName='+w.documentLocation+'@file">Download</a>'))
                                           
                                            .append($('<img>')
                                                //.attr('src', 'https://simple.wikipedia.org/wiki/Link#/media/File:Chain_link_icon.png')
                                                .attr('src', w.documentLocation)
                                                .attr('onclick', "window.open(this.src, '_blank')")
                                                .text('Image cell')
                                            )
                                        )
                                ); w.documentLocation
                            });
                        });
                        $("#nthRow").remove();
                    }
                    //else {
                    //    var docObj = result.result;
                    //    $("#DocumentTable tbody").find("tr:gt(0)").remove();

                    //    $('#DocumentTable').find('tbody')
                    //        .append('<tr><th>Id</th><th>DocumentPath</th></tr>');
                    //    $.each(docObj, function (i, v, row) {
                    //        $('#DocumentTable').find('tbody').append('<tr>');
                    //        $('#DocumentTable').find('tbody')
                    //            .append('<td id=' + i + 'id>' + v.requestedItemId + '</td>');
                    //        //.append('<td>');
                    //        $.each(v.itemDocuments, function (j, w) {
                    //            $('#DocumentTable').find('tbody')
                    //                .append('<label id=' + j + 'id>' + w.documentLocation + '</label>');
                    //        });
                    //        $('#DocumentTable').find('tbody')
                    //            .append('<tr height="3px;"><td></td></tr>');
                    //    });

                    //}

                },
                error: function (xhr, status, error) {
                    // alert(status);
                }
            });
        }
        alert($(this).val());
    });

})(jQuery);

//function getId(modelid) {
  
//    var fileData = new FormData();
//    fileData.append("Id", this.event.target.id);
//    fileData.append("Module", "IC");
//    fileData.append("RequestId", modelid);
//    var files1 = $('[id*=' + this.event.target.id + ']').get(0).files;
//    //var files1 = $(this.event.target.id).get(0).files;
//    for (var i = 0; i < files1.length; i++) {
//        fileData.append("fileInput", files1[i]);
//    }
//    if (fileData) {
//        $.ajax({
//            type: "POST",
//            //url: "/File/UploadFiles",
//            url: "/File/UploadFiles/",
//            dataType: "json",
//            contentType: false, // Not to set any content header
//            processData: false, // Not to process data
//            data: fileData,
//            success: function (result, status, xhr, data) {
//                console.log(result.result);
              
//                if (result.result == 'Error!' ) {
//                    $("#DocumentTable tbody").find("tr:gt(0)").remove();
//                }
//                else {
//                    var docObj = result.result;
//                    $("#DocumentTable tbody").find("tr:gt(0)").remove();

//                    $('#DocumentTable').find('tbody')
//                        .append('<tr><th>Id</th><th>DocumentPath</th></tr>');
//                    $.each(docObj, function (i, v, row) {
//                        $('#DocumentTable').find('tbody').append('<tr>');
//                        $('#DocumentTable').find('tbody')
//                            .append('<td id=' + i + 'id>' + v.requestedItemId + '</td>');
//                            //.append('<td>');
//                        $.each(v.itemDocuments, function (j, w) {
//                            $('#DocumentTable').find('tbody')
//                                .append('<label id=' + j + 'id>' + w.documentLocation + '</label>');
//                        });
//                        $('#DocumentTable').find('tbody')
//                            .append('<tr height="3px;"><td></td></tr>');
//                    });

//                }

//            },
//            error: function (xhr, status, error) {
//                // alert(status);
//            }
//        });
//    }
//}
