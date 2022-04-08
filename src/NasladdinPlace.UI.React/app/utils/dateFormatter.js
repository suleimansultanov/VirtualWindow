const normalizeByAddingLeadingZero = number =>
  number < 10 ? `0${number}` : number;

export function formatDate(date) {
  const day = date.getDate();
  const month = date.getMonth() + 1;
  const year = date.getFullYear();

  const formattedDay = normalizeByAddingLeadingZero(day);
  const formattedMonth = normalizeByAddingLeadingZero(month);

  return `${formattedDay}/${formattedMonth}/${year}`;
}
