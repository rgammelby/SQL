use Kinofest

create table Participant (
Participant_ID int identity(1, 1) primary key not null,  -- added identities
Name varchar(32) not null,
Email varchar(64) unique not null);  -- added unique constraint

create table Hall (
Hall_ID int identity(1, 1) primary key not null,
Seat_Amount tinyint not null,
check (Seat_Amount < 255));

create table Actor (
Actor_ID int identity(1, 1) primary key not null,
Name varchar(255) not null);

create table Director (
Director_ID int identity(1, 1) primary key not null,
Name varchar(255) not null);

create table Movie (
Movie_ID int identity(1, 1) primary key not null,
Release_Year int not null,
Duration int not null,
Genre varchar(50) not null,
Title varchar(255) not null,
Director_ID int foreign key references Director(Director_ID) not null,
Actor_ID int foreign key references Actor(Actor_ID) not null);

create table Cinema (
Cinema_ID int identity(1, 1) primary key not null,
Name varchar(255) not null,
Address varchar(255) not null,
Hall_Amount int not null,
Movie_ID int foreign key references Movie(Movie_ID) not null);

create table Showing (
Showing_ID int identity(1, 1) primary key not null,
Showing_Amount int not null,
Cinema_ID int foreign key references Cinema(Cinema_ID) not null,
Hall_ID int foreign key references Hall(Hall_ID) not null);

create table Ticket (
Ticket_ID int identity(1, 1) primary key not null,
Price decimal(6, 2) not null,
Showing_ID int foreign key references Showing(Showing_ID) not null,
Participant_ID int foreign key references Participant(Participant_ID) not null);

create table ActorsInMovies (
AIM_ID int identity(1, 1) primary key not null,
Movie_ID int foreign key references Movie(Movie_ID) not null,
Actor_ID int foreign key references Actor(Actor_ID) not null);

create table DirectorsOfMovies (
DOM_ID int identity(1, 1) primary key not null,
Movie_ID int foreign key references Movie(Movie_ID) not null,
Director_ID int foreign key references Director(Director_ID) not null);