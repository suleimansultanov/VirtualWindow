## Настройка и запуск
* Ставим ноду LTS не меньше десятой версии
* Устанавливаем yarn
`npm i -g yarn`
* В папке с проектом ставим зависимости
`yarn`
* Запускаем webpack-dev-server
`yarn watch`
* Либо одной командой
`yarn && yarn watch`


## Проверка на ошибки
* В режиме read-only
`yarn tslint`
* С автофиксом
`yarn tslint-fix`

*TSLint больше не поддерживается разработчиком, они настойчиво рекомендуют переехать на ESLint*
https://habr.com/ru/company/dodopizzadev/blog/473648/


## Сборка для разработки и прода
`yarn build-dev` (С сурс-мапами)
`yarn build-prod` (Минифицированный бандл без реактовских библиотек)