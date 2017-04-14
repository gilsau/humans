$(document).ready(function () {

    //Camera is clicked
    $('.glyphicon-camera').click(function () {
        $('#ProfilePic').trigger('click');
    });

    //Picture is selected to upload
    $('#ProfilePic').change(function () {
        
        var formData = new FormData();
        var totalFiles = document.getElementById("ProfilePic").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("ProfilePic").files[i];

            formData.append("ProfilePic", file);
        }
        $.ajax({
            type: "POST",
            url: '/account/updatephoto',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                
                //Update Profile Pics
                $('.roundImg, .roundImg2').attr('src', '../profilepics/' + response.DynObject);
                $('.bgProfile').css({ backgroundImage: 'url(../profilepics/' + response.DynObject + ')' });
            },
            error: function (error) {
                console.log("Error");
                console.log(error);
            }
        });

    });


    var PricePerHrMin = Number($('#sliderRate').attr('data-PricePerHrMin')) == 0 ? 5 : Number($('#sliderRate').attr('data-PricePerHrMin'));
    var PricePerHrMax = Number($('#sliderRate').attr('data-PricePerHrMax')) == 0 ? 20 : Number($('#sliderRate').attr('data-PricePerHrMax'));

    $("#sliderRate").slider({
        range: true,
        min: 5,
        max: 100,
        values: [PricePerHrMin, PricePerHrMax],
        slide: function (event, ui) {
            $("#rateDisplay").text('$' + ui.values[0] + " - $" + ui.values[1]);
        }
    });
    $("#rateDisplay").text('$' + $("#sliderRate").slider("values", 0) + " - $" + $("#sliderRate").slider("values", 1));

    //Save changes
    $('.save').click(function () {

        var loading = $('.save img');
        var oksign = $('.save span.ok');
        var errsign = $('.save span.err');

        //collect all values
        var formData = new FormData();
        formData.append("FirstName", $('#FirstName').val());
        formData.append("MiddleName", $('#MiddleName').val());
        formData.append("LastName", $('#LastName').val());
        formData.append("City", $('#City').val());
        formData.append("StateId", $('#StateId').val());
        formData.append("Zipcode", $('#Zipcode').val());
        formData.append("UserName", $('#UserName').val());
        formData.append("Password", $('#Password').val());
        formData.append("ConfirmPassword", $('#ConfirmPassword').val());
        formData.append("Gender", $('[name="Gender"]:checked').val());
        formData.append("Birthdate", $('#Birthdate').val());
        formData.append("Title", $('#Title').val());
        formData.append("Company", $('#Company').val());
        formData.append("CompanyAddress1", $('#CompanyAddress1').val());
        formData.append("CompanyAddress2", $('#CompanyAddress2').val());
        formData.append("CompanyCity", $('#CompanyCity').val());
        formData.append("CompanyStateId", $('#CompanyStateId').val());
        formData.append("CompanyZipcode", $('#CompanyZipcode').val());
        formData.append("CompanyPhone", $('#CompanyPhone').val());
        formData.append("PricePerHrMin", $("#sliderRate").slider("values", 0));
        formData.append("PricePerHrMax", $("#sliderRate").slider("values", 1));
        formData.append("Paypal", $('#Paypal').val());
        formData.append("NameOnCard", $('#NameOnCard').val());
        formData.append("CreditCardNo", $('#cc1').val().toString().concat($('#cc2').val(), $('#cc3').val(), $('#cc4').val()));
        formData.append("ExpDateMo", $('#ExpDateMo').val());
        formData.append("ExpDateYr", $('#ExpDateYr').val());
        formData.append("CardType", $('[name="CardType"]:checked').val());

        console.log('formData');
        console.log(formData);

        //Show dialog for at least 2 seconds
        loading.show();
        oksign.hide();
        errsign.hide();
        setTimeout(function () {

            //Submit
            $.ajax({
                type: "POST",
                url: "/account/myprofile",
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false
            })
            .fail(function (response) {
                loading.hide();
                errsign.show();
                errsign.text(response);
            })
            .success(function (response) {
                loading.hide();
                oksign.show();
            });

        }, 2000);

    });
});