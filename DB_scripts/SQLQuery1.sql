create table website(ID uniqueidentifier not null,Url nvarchar(max) not null,xpath nvarchar(max) null,constraint Pk_website primary key clustered(id Asc));
alter database websitewatcher set change_tracking=ON
alter table website enable change_tracking;
create table snapshot(ID uniqueidentifier not null,Content nvarchar(max) not null,constraint Pk_snapshot primary key clustered(id Asc));
select * from website;
select * from snapshot;
delete website;
delete snapshot;
alter table snapshot add Timestamp Datetime not null Default getutcdate();
alter table website add Timestamp Datetime not null Default getutcdate();
Alter table snapshot drop constraint Pk_snapshot
alter table snapshot add constraint Pk_snapshot Primary key (id,timestamp)





