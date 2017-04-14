var convoLst, convoBox, convoBox2, convoid_current, convo_timer, addBox, txtBox, addTxtBox, sugBox, convoListLink, convoTitle, convoLeftPanel, convoRightPanel;

$(document).ready(function () {
    convoLst = $('#convoList');
    convoBox = $('#convoBox');
    convoBox2 = $('#convoBox2');
    convoListLink = $('#convoListLink');
    convoTitle = $('#convoTitle');
    convoLeftPanel = $('#convoLeftPanel');
    convoRightPanel = $('#convoRightPanel');
    addBox = $('#addBox');
    txtBox = $('#txtMsg');
    addTxtBox = addBox.find('input');
    sugBox = $('#suggestBox');
    
    //Suggest users
    $('#addBox input').keypress(function () {
        SuggestUsers($(this));
    });

    //Start new conversation
    $('#btnAdd').click(function () {
        addTxtBox.val('');
        addBox.toggle('slide');
        sugBox.hide();
        addTxtBox.focus();
    });

    //Send msg
    $('#btnSend').click(function () {
        AddMsgToConvo();
    });

    //Resize sections
    $(window).resize(function () {
        ResizeConvoSections();
    });

    //Back to conversations list
    $('.glyphicon-th-list').click(function () {
        convoRightPanel.hide();
        convoLeftPanel.show();
    });

    //Send msg to user from other page
    var idToSendTo = getUrlParameter('id');
    if (getUrlParameter('add') && idToSendTo) {
        convoid_current = ''
        AddMsgToConvo(idToSendTo);
    }

    //On page load
    ResizeConvoSections();
    LoadConversations();
});

function SuggestUsers(sugTxtBox) {
    setTimeout(function () {
        $.ajax({
            type: "GET",
            url: "/account/getsuggestions",
            data: { partialname: sugTxtBox.val() },
            dataType: 'json'
        })
        .fail(function (response) {
            console.log('Error');
            console.log(response);
        })
        .success(function (response) {
            //console.log('Success');
            //console.log(response);

            sugBox.html('');
            if (response.Success) {
                var users = response.DynObject;

                //Add suggested users to box
                for (var u in users) {
                    var user = users[u];
                    sugBox.append('<div class="suggestItem" data-id="' + user.UserId + '"><span class="btn btn-sm">' + user.FirstName + '</span></div>');
                }

                //Show box with suggested users
                if (users.length > 0) {
                    sugBox.show();
                }

                //Event handler for click, for suggested user
                $('.suggestItem').click(function () {
                    convoid_current = '';
                    AddMsgToConvo($(this).attr('data-id'));
                });
            }
        });
    }, 1000);
}

function AddMsgToConvo(toId) {
    var msg = txtBox.val();
    
    txtBox.val('');
    addTxtBox.val('');
    addBox.hide();
    sugBox.hide();
    
    $.ajax({
        type: "POST",
        url: "/messages/add",
        data: { convoid: convoid_current, msg: msg, toId: (toId ? toId : 0) },
        dataType: 'json'
    })
    .fail(function (response) {
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        console.log('Success');
        console.log(response);

        if (response.Success) {
            if (convoid_current == '') { convoid_current = response.DynObject; }
            GetLoadConversation(convoid_current);

            var winWd = $(window).width();
            if (winWd <= widthLimit) {
                convoLeftPanel.hide();
                convoRightPanel.show();
            }
        }
    });
}

function GetLoadConversation(convoid) {
    var winWd = $(window).width();
    
    //Get convo
    $.ajax({
        type: "GET",
        url: "/messages/get",
        data: { convoid: convoid },
        dataType: 'json'
    })
    .fail(function (response) {
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        //console.log('Success');
        //console.log(response);

        if (response.Success) {
            var mhtml = '';
            var msgs = response.DynObject;
            var otherperson = '';
        
            for (var m in msgs) {
                var msg = msgs[m];
                var prefix = msg.From.Me ? 'me' : msg.From.FirstName.toLowerCase();
                otherperson = msg.From.Me ? msg.To.FirstName.toLowerCase() : msg.From.FirstName.toLowerCase();

                if ($.trim(msg.Msg) != '') {
                    mhtml += '<div style="border-bottom:solid 1px #eee;padding:5px;"><table style="width:100%;"><tr>' +
                                '<td style="width:50%;" class="text-left list-group-item-heading" style="font-size:14px;"><b>' + prefix + '</b></td>' +
                                '<td style="width:50%;" class="text-right">' + TimePassed(msg.Days, msg.Hours, msg.Minutes, msg.Seconds) + '</td>' +
                            '</tr><tr>' +
                                '<td colspan="2" class="list-group-item-text" style="font-size:12px;padding-top:3px;">' + msg.Msg + '</td>' +
                            '</tr></table></div>';
                }
            }
            convoBox2.html(mhtml);

            //For small devices
            if (winWd <= widthLimit) {
                convoTitle.text(otherperson);
            }

            //Scroll down to see latest msg
            convoBox2.scrollTop(convoBox2[0].scrollHeight);

            //Enable response box
            $('#responseSect').show();

            //Set timer for conversation updates
            if (convoid != convoid_current) {
                convoid_current = convoid;

                clearInterval(convo_timer);
                convo_timer = setInterval(function () {
                    GetLoadConversation(convoid_current);
                }, 10000);
            }
        }

        //Reload convos with new statuses
        LoadConversations();
    });
}

function ResizeConvoSections() {
    var winHt = $(window).height();
    var winWd = $(window).width();
    var headHt = $('header').height() + 80;
    
    //Convo list
    convoLst.css('height', winHt - 55 - headHt);

    //Convo box
    if (winWd <= widthLimit) {
        convoBox.css('height', winHt - headHt - 60);
    }
    else {
        convoBox.css('height', winHt - headHt);
    }
}

function LoadConversations() {
    $.ajax({
        type: "GET",
        url: "/messages/get",
        data: { convoid:'all' },
        dataType: 'json'
    })
    .fail(function (response) {
        console.log('Error');
        console.log(response);
    })
    .success(function (response) {
        //console.log('Success');
        //console.log(response);
        
        if (response.Success) {
            var chtml = '';
            var convos = response.DynObject;
        
            for (var c in convos) {
                var convo = convos[c];
                var pic = convo.To.Me ? convo.From.Pic : convo.To.Pic;
                var first = convo.To.Me ? convo.From.FirstName.toLowerCase() : convo.To.FirstName.toLowerCase();
                var last = convo.To.Me ? convo.From.LastName.toLowerCase() : convo.To.LastName.toLowerCase();
                var prefix = convo.From.Me ? 'me:' : convo.From.FirstName.toLowerCase() + ':'; if ($.trim(convo.Msg) == '') { prefix = ''; }
                var newicon = convo.Status == 'New' && convo.To.Me ? '<span class="glyphicon glyphicon-envelope pull-right"></span>' : '';
                
                chtml += 
                    '<a data-convoid="' + convo.ConvoId + '" class="convolink btn list-group-item" style="text-align:left;">' +
                        '<table style="width:100%;"><tr>' +
                            '<td style="width:50%;" class="text-left list-group-item-heading" style="font-size:14px;"><img class="roundImg5" src="../profilepics/' + pic + '"/> <b>' + first + ' ' + last + '</b></td>' +
                            '<td style="width:50%;" class="text-right">' + TimePassed(convo.Days, convo.Hours, convo.Minutes, convo.Seconds) + '<br/>' + newicon + '</td>' +
                        '</tr><tr>' +
                            '<td colspan="2" class="list-group-item-text" style="font-size:12px;padding-top:3px;"><b>' + prefix + '</b> ' + convo.Msg + '</td>' +
                        '</tr></table>' +
                    '</a>';
            }
            convoLst.html(chtml);

            //Highlight current conversation
            if (convoid_current) {
                $('[data-convoid="' + convoid_current + '"]').addClass('active whitetext');
            }

            //conversation link
            $('.convolink').click(function () {
                var link = $(this);
                GetLoadConversation(link.attr('data-convoid'));

                var winWd = $(window).width();
                if (winWd <= widthLimit) {
                    convoLeftPanel.hide();
                    convoRightPanel.show();
                }
            });

            txtBox.focus();
        }
    });
}

function TimePassed(days, hours, mins, secs)
{
    var parts = 0;
    var timep = '';
    
    if (days > 0) {
        timep += days + ' days ';
        parts++;
    }

    if (hours % 24 > 0) {
        timep += hours % 24 + ' hrs ';
        parts++;
    }

    if (mins % 60 > 0 && parts < 2) {
        timep += mins % 60 + ' mins ';
        parts++;
    }

    if (secs % 60 > 0 && parts < 2) {
        timep += secs % 60 + ' secs ';
        parts++;
    }

    return $.trim(timep) == '' ? '1 sec' : timep;
}

function getUrlParameter(sParam)
{
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) 
    {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) 
        {
            return sParameterName[1];
        }
    }
}   