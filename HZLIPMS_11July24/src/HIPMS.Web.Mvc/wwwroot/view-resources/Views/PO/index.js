(function ($) {
    $('#userDiv').hide();
    $('#btnSubmit').hide();
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
    $('#createMap').on('submit', function (e) {
        
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

    //$(".js-example-placeholder-single").select2({
    //    placeholder: "Select a state",
    //    allowClear: true
    //});
})(jQuery);
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

$("#POSearch").change(function () {
    $('#userDiv').hide();
    $('#btnSubmit').hide();
});
$("#btnSearch").on("click", function (event) {
  
    var pOText = $("#POSearch").val();
 
    $.ajax({
        url: abp.appPath + 'PO/POSearch/?ponumber=' + pOText,
        data: 'ponumber=' + pOText,
        type: "Post",
        contentType: "application/json"
    }).done(function (data) {
        if (data && data.result) {
            if (data.result) {
                $('#userDiv').show();
                $('#btnSubmit').show();
            }
        }

    }).fail(function (e, jqXHR, textStatus, errorThrown) {

        $('#userDiv').hide();
      
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



