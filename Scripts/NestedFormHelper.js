function removeNestedForm(element, container, deleteElement) {
    $container = $(element).parents(container);
    //$container.find(deleteElement).remove();
    $container.remove();
}


function addNestedForm(container, counter, ticks, content_id, func) {

    var content = $("#" + content_id).html();
    var index = $(container).data("count");
    if (index == undefined) index = $(container + " " + counter).length;
    var pattern = new RegExp(ticks, "gi");
    content = content.replace(pattern, index);
    pattern = new RegExp("nf_script_clone", "gi");
    content = content.replace(pattern, "script");
    
    //content = $(content).find(counter).data("index",index);
    
    $(container).append(content);
    $(container).data("count", index + 1);
    if (func) {
        func(this);
    }
}