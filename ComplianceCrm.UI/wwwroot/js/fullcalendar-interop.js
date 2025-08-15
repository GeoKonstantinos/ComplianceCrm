window.complianceCalendar = {
    init: function (elem, events, dotnet, options) {
        const cfg = Object.assign({
            initialView: 'timeGridWeek',
            slotDuration: '00:30:00',
            nowIndicator: true,
            editable: true,           // drag & drop / resize
            selectable: true,
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            eventDrop: function (info) {
                dotnet.invokeMethodAsync('OnEventDrop',
                    info.event.id,
                    info.event.start ? info.event.start.toISOString() : null,
                    info.event.end ? info.event.end.toISOString() : null)
                    .catch(() => info.revert());
            },
            eventResize: function (info) {
                dotnet.invokeMethodAsync('OnEventResize',
                    info.event.id,
                    info.event.start ? info.event.start.toISOString() : null,
                    info.event.end ? info.event.end.toISOString() : null)
                    .catch(() => info.revert());
            },
            eventClick: function (info) {
                dotnet.invokeMethodAsync('OnEventClick', info.event.id);
            },
            events: events
        }, options || {});

        const calendar = new FullCalendar.Calendar(elem, cfg);
        calendar.render();
        elem._calendar = calendar;
    },

    setEvents: function (elem, events) {
        const cal = elem._calendar;
        if (!cal) return;
        cal.removeAllEvents();
        cal.addEventSource(events);
    },

    destroy: function (elem) {
        const cal = elem._calendar;
        if (cal) { cal.destroy(); elem._calendar = null; }
    }
};
