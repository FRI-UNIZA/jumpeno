namespace Jumpeno.Server.Constants;

public enum ISOLATION {
    READ_UNCOMMITED = System.Data.IsolationLevel.ReadUncommitted,
    READ_COMMITED = System.Data.IsolationLevel.ReadCommitted,
    REPEATABLE_READ = System.Data.IsolationLevel.RepeatableRead,
    SERIALIZABLE = System.Data.IsolationLevel.Serializable
}
