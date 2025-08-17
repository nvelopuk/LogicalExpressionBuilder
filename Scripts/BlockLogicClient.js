var blockLogicClient = (function () {
    return {
        config: {},
        selector: '.output',
        init: function (selector) {

            if (selector)
                blockLogicClient.selector = selector;

            $(document).on('click', '.action', function (e) {

                var btn = $(e.target).clone();
                var type = btn.data('type');
                var tpl = null;

                tpl = $('.' + (type.toLowerCase() == 'statement' ? btn.val().toLowerCase().replace(/ /g, '') : type.toLowerCase()) + '.template').clone();
                tpl.removeClass('template');

                tpl.find('input').val(btn.val());

                if (type == 'input') {
                    tpl.find('input').attr('type', btn.val());
                }
                else if (type != 'statement') {
                    tpl.find('button').html(tpl.find('button').html().replace('[]', btn.text()));
                    tpl.find('button').val(btn.val());
                }


                if ($('.selected').length > 0) {

                    var maxOptions = $('.selected').data('max-options');

                    if (maxOptions > 0 && $('.selected *').length >= maxOptions)
                        return;

                    $('.selected').append(tpl);
                }
                else {
                    $('.output').append(tpl);
                }
            });

            $(document).on('click', '.remove', function (e) {

                $(e.currentTarget).closest('li').remove();
                $("ul.output").sortable('refresh');
            });

            $(document).on('click', '.inner .target', function (e) {
                var fs = $(e.currentTarget);
                var allowedOptions = JSON.parse(fs.data('allowed-options').replace(/\'/g, '"'));

                $('.target').removeClass('selected');

                $('.output .target').each(function (i) {

                    if ($(this).html() == '')
                        $(this).html($(this).data('default-text'));
                });

                fs.addClass('selected');

                if (fs.html().indexOf('[') == 0)
                    fs.html('');
                e.stopPropagation();

                $('button.action').attr('disabled', true);

                $.each(allowedOptions, function () {

                    var opt = this;

                    if (opt.indexOf(':') > 0) {
                        var opts = opt.split(':');
                        $('button.action.' + opts[0] + '[value=' + opts[1] + ' i]').removeAttr('disabled');
                    }
                    else {
                        $('button.action.' + opt.replace(' ', '')).removeAttr('disabled');
                    }
                });
            });

            $(document).on('click', '.import', function (e) {
                importData(blockLogicClient.config.expression);
            });

            $(document).on('click', '.export', function (e) {
                exportData($('.output-container'));
            });

            $(document).on('click', '.clear', function (e) {
                blockLogicClient.config.expression = {};

                $('.output').empty();
            });

            $(document).on('click', '.reset', function (e) {
                importData(blockLogicClient.config.expression);
            });

            $(document).on('click', '.save-server', function (e) {
                exportData($('.output-container'));
                saveToServer();
            });

            function importData(data) {

                console.log(data);

                $('.output').empty();

                var html = '';

                $.each(data.nodes, function (i) {
                    var tempNode = this;
                    html += loadNode(tempNode);
                });

                addToBuilder(html);
            }

            function loadNode(node, sel) {

                console.log(node.type);
                console.log($(sel));
                var el = $('<li></li>');

                if (node.type == "if") {
                    loadIfNode(node, sel);
                }
                else if (node.type == "criteria") {
                    loadCriteriaNode(node, sel);
                }
                else if (node.type == "groupstatement") {
                    loadGroupStatementNode(node, sel);
                }
                else if (node.type == "mathsoperation") {
                    loadMathsOperationNode(node, sel);
                }
                else if (node.type == "then") {
                    loadThenNode(node, sel);
                }
                else if (node.type == "else") {
                    loadElseNode(node, sel);
                    console.log('else ' + sel);
                }
                else if (node.type == "left") {
                    loadNode(node.nodes[0], sel);
                }
                else if (node.type == "right") {
                    $.each(node.nodes, function () {
                        loadNode(this, sel);
                    });
                }
                else if (node.type == "group") {
                    loadGroupNode(node, sel);
                }
                else if (node.type == "expression") {
                    loadExpressionNode(node, sel);
                }
                else {
                    //when dealing with operator and other nodes, we need to go down another nested level to get the value (like most nodes)
                    if ($('.' + node.type.toLowerCase() + '.template').length > 0) {
                        el = $('.' + node.type.toLowerCase() + '.template').clone();
                        el.removeClass('template');

                        if (el.find('button').length > 0) {
                            el.find('button').html(el.find('button').html().replace('[]', (node.name ? node.name : node.nodes[0].name)));
                            el.find('button').val(node.value ? node.value : node.nodes[0].value);
                        }
                        else {
                            el.find('input').attr('type', node.inputType.toLowerCase());
                            el.find('input').val((node.value) ? node.value : node.value);
                        }

                        addToBuilder(el, sel);
                    }
                    else {
                        if (!node.type)
                            return;

                        $(el).text(node.type);

                        if (el.find('ul').length == 0) {
                            el.append('<ul></ul>');
                        }

                        addToBuilder(el, tgt);
                    }


                }



            }

            function loadGroupStatementNode(node, sel) {
                $.each(node.nodes, function () {
                    loadNode(this, sel);
                });
            }

            function loadIfNode(node, sel) {

                var tpl = $('.' + node.type.toLowerCase() + '.template').clone();

                tpl.removeClass('template');

                addToBuilder(tpl, sel);

                loadNode(arrayLookup(node.nodes, 'type', 'criteria'), tpl.getPath() + ' > .inner > ul[data-type=criteria]');
                loadNode(arrayLookup(node.nodes, 'type', 'then'), tpl.getPath() + ' > .inner > ul[data-type=then]');
                loadNode(arrayLookup(node.nodes, 'type', 'else'), tpl.getPath() + ' > .inner > ul[data-type=else]');

                console.log(tpl.find(tpl.getPath() + ' ul[data-type=else]'));
            }

            function loadThenNode(node, sel) {
                $.each(node.nodes, function () {
                    loadNode(this, sel);
                });
            }

            function loadElseNode(node, sel) {
                $.each(node.nodes, function () {
                    loadNode(this, sel);
                });
            }

            function loadGroupNode(node, sel) {
                var tpl = $('.' + node.type.toLowerCase() + '.template').clone();
                tpl.removeClass('template');

                addToBuilder(tpl, sel);

                loadNode(arrayLookup(node.nodes, 'type', 'condition'), tpl.getPath() + ' > .inner > ul[data-type=condition]');
                loadNode(arrayLookup(node.nodes, 'type', 'groupstatement'), tpl.getPath() + ' > .inner > ul[data-type=groupstatement]');
            }

            function loadExpressionNode(node, sel) {
                var tpl = $('.' + node.type.toLowerCase() + '.template').clone();
                tpl.removeClass('template');

                addToBuilder(tpl, sel);

                loadNode(arrayLookup(node.nodes, 'type', 'left'), tpl.getPath() + ' > .inner > ul[data-type=left]');
                loadNode(arrayLookup(node.nodes, 'type', 'operator'), tpl.getPath() + ' > .inner > ul[data-type=operator]');
                loadNode(arrayLookup(node.nodes, 'type', 'right'), tpl.getPath() + ' > .inner > ul[data-type=right]');
            }

            function loadMathsOperationNode(node, sel) {
                var tpl = $('.' + node.type.toLowerCase() + '.template').clone();
                tpl.removeClass('template');

                addToBuilder(tpl, sel);

                loadNode(arrayLookup(node.nodes, 'type', 'left'), tpl.getPath() + ' > .inner > ul[data-type=left]');
                loadNode(arrayLookup(node.nodes, 'type', 'operator'), tpl.getPath() + ' > .inner > ul[data-type=operator]');
                loadNode(arrayLookup(node.nodes, 'type', 'right'), tpl.getPath() + ' > .inner > ul[data-type=right]');
            }

            function loadCriteriaNode(node, sel) {

                $.each(node.nodes, function () {
                    loadNode(this, sel);
                });
            }

            function addToBuilder(tpl, sel) {


                if (tpl == 'undefined')
                    return;

                if (sel) {

                    var target = $(sel);

                    if (target.length > 1) {
                        console.log(target);
                    }

                    if (target.text().indexOf('[') == 0)
                        target.text('');

                    target.append(tpl);

                    return;
                }

                if ($('.selected').length > 0) {
                    $('.selected').append(tpl);
                }
                else {
                    $('.output').append(tpl);
                }
            }

            function exportData(el) {
                //need to recursivel work through each node and extract an object
                var sel = $('ul.output > li');

                blockLogicClient.config.expression = { nodes: [] };

                sel.each(function (i) {
                    var obj = exportNode($(this));

                    blockLogicClient.config.expression.nodes.push(obj);
                });

                $('.data-output').empty();

                $('.data-output').text(JSON.stringify(blockLogicClient.config.expression, null, 4));
            }

            function exportNode(el) {

                if ($(el).children('.inner').children('ul.target').length == 0)
                    return;

                var objs = { type: el.data('type'), nodes: [] };

                $(el).children('.inner').children('ul.target').each(function (i) {
                    var curNode = $(this);
                    var obj = { type: curNode.data('type'), nodes: [] };

                    curNode.children('.outer').each(function () {
                        var innerObj = exportNode($(this));

                        if (innerObj)
                            obj.nodes.push(innerObj);
                    });


                    $(this).children('.outer').children('button').each(function (i) {
                        obj.nodes.push({ name: $(this).text().trim(), value: $(this).val().replace(/ /g, ''), type: $(this).data('type') });
                    });

                    $(this).children('.outer').children('.inner').children('input').each(function (i) {
                        obj.nodes.push({ value: $(this).val(), type: $(this).data('type'), inputType: $(this).attr('type').toLowerCase() });
                    });

                    objs.nodes.push(obj);
                });

                return objs;
            }

            function buildOptions(callback) {

                var div = $('<div class=""></div>');

                $('div.options').append(div);

                var colours = ['success', 'danger', 'warning', 'primary', 'secondary', 'info', 'light', 'dark', 'muted'];

                var i = 0;

                $.each(blockLogicClient.config.options, function () {

                    var optType = this;

                    var div = $('<div class="' + optType.name + '"></div>');

                    $('div.options').append(div);

                    $.each(optType.options, function () {
                        div.append('<button type="button" class="' + optType.type + ' action btn btn-xs btn-' + colours[i] + '" data-type="' + optType.type + '" value="' + (this.value ? this.value : this.name) + '">' + this.name + '</button>');
                    });

                    i++;
                });

                if (callback)
                    callback();

            }

            function arrayLookup(array, prop, val) {
                for (var i = 0, len = array.length; i < len; i++) {
                    if (array[i].hasOwnProperty(prop) && array[i][prop] === val) {
                        return array[i];
                    }
                }

                return null;
            }

            function saveToServer() {

                $.ajax({
                    type: "POST",
                    url: '/ProcessLogic/FromBlockClient/?questionid=' + $('#QuestionId').val() + '&logictype=' + $('#LogicType').val(),
                    contentType: "application/json; charset=utf-8",
                    datatype: 'json',
                    data: JSON.stringify(blockLogicClient.config.expression),
                    success: function (data) {
                        $('.server-response').html(data.value).show();
                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }

            buildOptions(function () {
                if (blockLogicClient.config.expression)
                    importData(blockLogicClient.config.expression);
            });



        }
    };
})();

jQuery.fn.extend({
    getPath: function () {
        var pathes = [];

        this.each(function (index, element) {
            var path, $node = jQuery(element);

            while ($node.length) {
                var realNode = $node.get(0), name = realNode.localName;
                if (!name) { break; }

                name = name.toLowerCase();
                var parent = $node.parent();
                var sameTagSiblings = parent.children(name);

                if (sameTagSiblings.length > 1) {
                    var allSiblings = parent.children();
                    var index = allSiblings.index(realNode) + 1;
                    if (index > 0) {
                        name += ':nth-child(' + index + ')';
                    }
                }

                path = name + (path ? ' > ' + path : '');
                $node = parent;
            }

            pathes.push(path);
        });

        return pathes.join(',');
    }
});