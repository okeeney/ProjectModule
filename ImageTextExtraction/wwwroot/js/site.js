// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$("#aws_upload_form").submit(function (e) {
	e.preventDefault();
	the_file = this.elements['file'].files[0]; //get the file element
	var filename = Date.now() + '.' + the_file.name.split('.').pop(); //make file name unique using current time (milliseconds)
	$(this).find("input[name=key]").val(filename); //key name 
	$(this).find("input[name=Content-Type]").val(the_file.type); //content type

	var post_url = $(this).attr("action"); //get form action url
	var form_data = new FormData(this); //Creates new FormData object
	$.ajax({
		url: post_url,
		type: 'post',
		datatype: 'xml',
		data: form_data,
		contentType: false,
		processData: false,
		xhr: function () {
			var xhr = $.ajaxSettings.xhr();
			if (xhr.upload) {
				var progressbar = $("<div>", { style: "background:#607D8B;height:10px;margin:10px 0;" }).appendTo("#results"); //create progressbar
				xhr.upload.addEventListener('progress', function (event) {
					var percent = 0;
					var position = event.loaded || event.position;
					var total = event.total;
					if (event.lengthComputable) {
						percent = Math.ceil(position / total * 100);
						progressbar.css("width", + percent + "%");
					}
				}, true);
			}
			return xhr;
		}
	}).done(function (response) {
		var url = $(response).find("Location").text(); //get file location
		var the_file_name = $(response).find("Key").text(); //get uploaded file name
		$("#results").html("<span>File has been uploaded, Here's your file <a href=" + url + ">" + the_file_name + "</a></span>"); //response
	});
});
