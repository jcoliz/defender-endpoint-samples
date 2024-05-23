INSERT INTO "UpdateAlertTasks" ("Action", "Payload", "Status", "SubjectId")
VALUES (1, "Hello", 0, 1)
RETURNING "Id";