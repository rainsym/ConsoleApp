create view [dbo].[UnsubscribeEvent]
as
select re.Id, re.Name, re.Subscriber as Register, _temp.Subscriber as Real, _temp.MessageId, PayLoad, _temp.CreatedAt
from RawRabbitEvent re
inner join
(
	select * from
	(
		select MessageId, Subscriber, Name, PayLoad, CreatedAt from EventTracker where MessageId in(
		select MessageId
		from EventTracker
		group by MessageId
		having count(*) < (select count(*) from RawRabbitEvent group by Name) + 1) and Subscriber is not null
	)_tbl1
	union
	select * from
	(
		select MessageId, Subscriber, Name, PayLoad, CreatedAt from EventTracker where MessageId in(
		select MessageId
		from EventTracker
		group by MessageId
		having count(*) = 1)
	)_tbl2
)_temp on _temp.Name = re.Name
where _temp.Subscriber is null or re.Subscriber <> _temp.Subscriber