CREATE TABLE users (
	id int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	login varchar(64) UNIQUE NOT NULL,
	password varchar(256) NOT NULL
);

CREATE TABLE messages (
	id int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	content nvarchar(2048) NULL,
	user_id int NOT NULL,

	-- constraint user_id
);