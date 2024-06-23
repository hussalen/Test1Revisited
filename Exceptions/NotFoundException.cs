namespace GakkoHorizontalSlice.Exceptions;

public abstract class NotFoundException(string message) : Exception(message);

public class ProjectNotFound(int id) : NotFoundException($"Project with id {id} not found");

public class TeamMemberNotFound(int id) : NotFoundException($"Team Member with id {id} not found");

public class TaskTypeNotFound(int id) : NotFoundException($"Task Type with id {id} not found");
