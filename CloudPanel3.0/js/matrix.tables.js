
$(document).ready(function(){
	
	$('.data-table').dataTable({
	    "bJQueryUI": true,
	    "bInfo": true,
        "aLengthMenu": [10, 50, 100, 500, 1000, 2000],
        "sPaginationType": "full_numbers",
        "iDisplayLength": 50,
        "bSort": false,
		"sDom": '<""li>t<"F"fp>'
	});
	
	$('input[type=checkbox],input[type=radio],input[type=file]').uniform();
	
	$('select').select2();
	
	$("span.icon input:checkbox, th input:checkbox").click(function() {
		var checkedStatus = this.checked;
		var checkbox = $(this).parents('.widget-box').find('tr td:first-child input:checkbox');		
		checkbox.each(function() {
			this.checked = checkedStatus;
			if (checkedStatus == this.checked) {
				$(this).closest('.checker > span').removeClass('checked');
			}
			if (this.checked) {
				$(this).closest('.checker > span').addClass('checked');
			}
		});
	});	
});
