create procedure GetStudentByDepartment
	@Department nvarchar(50)
as
begin
	select * from Students where Department = @Department
end

