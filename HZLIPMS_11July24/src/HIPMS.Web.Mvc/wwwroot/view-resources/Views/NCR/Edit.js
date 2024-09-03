(function ($) {
    $(document).ready(function () {

    })
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
    $('[id^="fileInput"]').change(function (evt) {
        var fileData = new FormData();
        fileData.append("Id", evt.target.id);
        fileData.append("Module", "NCR");
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
                        $("#NCRDocumentTable tbody").find("tr:gt(0)").remove();
                    }
                    else {
                        var docObj = result.result;
                        $("#NCRDocumentTable tbody").find("tr:gt(0)").remove();

                       
                        $.each(docObj, function (i, v, row) {

                            $.each(v.itemDocuments, function (j, w) {

                                onclick = "window.open(this.src, '_blank');"

                                $("#NCRDocumentTable").last()
                                    .append($('<tr>').append('<td id=' + i + 'id>' + v.requestedItemId + '</td>')
                                        .append($('<td>')
                                            .append($('<img>')
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
                   
                },
                error: function (xhr, status, error) {
                    alert(error);
                }
            });
        }
        //alert($(this).val());
    });

})(jQuery);