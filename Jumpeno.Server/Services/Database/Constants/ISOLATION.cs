namespace Jumpeno.Server.Enums;

public enum Isolation {
    ReadUncomitted = System.Data.IsolationLevel.ReadUncommitted,
    ReadCommitted = System.Data.IsolationLevel.ReadCommitted,
    RepeatableRead = System.Data.IsolationLevel.RepeatableRead,
    Serializable = System.Data.IsolationLevel.Serializable
}
