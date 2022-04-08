/* eslint-disable no-undef */
export function showSuccess() {
  toastr.success('Операция прошла успешно.', '');
}

export function showError() {
  toastr.error('Упсс! Произошла ошибка.');
}
