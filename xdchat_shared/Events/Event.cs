namespace XdChatShared.Events {
    /* => Event System <=
     *
     * Events
     * => Events must extend Event
     * => Events can extend IEventFilter to use the Filter Property in XdEventHandlers
     * => Events can extend IAnonymousContextProvider so they can be scoped to a module context
     *
     * EventListeners
     * => Classes extending IEventListener can contain event handlers
     * => Event handlers are methods that must be attributed with XdEventHandler
     * => The first parameter of an event handler method must be a class extending Event
     *
     * EventEmitters
     * => Used to emit events and register event listeners
     */
    public class Event {}
}