namespace MyAssetsManagerBackend.enums;

public enum EntityStatus
{
    // video project statuses
    DRAFT, PENDING, TEST, APPROVED, DELETED, ARCHIVED,
	
    // purl and render job statuses
    NEW, TRANSFORMED, RENDERING, RENDERED, ERROR, SUSPENDED, COPY, ADDED, REQUEUED, MAKING, READY,
	
    // other statuses
    PREPARING, FINISHING, DONE, FAILED, DISABLED, PROCESSING,
	
    NONE
}