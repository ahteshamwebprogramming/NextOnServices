-- =============================================
-- Script to Create Database: 08Consultings
-- =============================================
-- This script creates the database if it doesn't exist
-- Run this script first before creating tables

-- Check if database exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '08Consultings')
BEGIN
    CREATE DATABASE [08Consultings]
    PRINT 'Database 08Consultings created successfully.'
END
ELSE
BEGIN
    PRINT 'Database 08Consultings already exists.'
END
GO

USE [08Consultings]
GO

