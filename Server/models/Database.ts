import Knex from 'knex';

const knex = Knex({
  client: 'sqlite3',
  connection: {
    filename: './database.db',
  },
  useNullAsDefault: true,
});

export class Database {
  table: string;
  knex: Knex;

  constructor(table: string) {
    this.table = table;
    this.knex = knex;
  }

  async safe() {
    await this.ensureTable();
    return this;
  }

  async ensureTable() {
    let exists = await this.knex.schema.hasTable(this.table);
    if (exists) return;
    let builder = keyValueSchema;
    if (this.table == 'players') builder = playerSchema;
    else if (this.table == 'links') builder = linksSchema;
    await this.knex.schema.createTable(this.table, builder);
    console.log(`Creating table ${this.table}`);
  }

  async get(key: string, value: string) {
    let rows = await this.knex.select('*').from(this.table).where(key, value);

    if (!rows.length) return [];

    return rows;
  }

  async getTop5(key: string) {
    return await this.knex
      .select('*')
      .from('players')
      .orderBy(key, 'desc')
      .limit(5);
  }
  async getFirst(key: string, value: string) {
    return (await this.get(key, value))[0];
  }

  async getJSON(key) {
    return JSON.parse((await this.getFirst('key', key)).value);
  }

  async getKeyArray(keyArray: Array<string>, value: string) {
    for (var key in keyArray) {
      var data = await this.getFirst(keyArray[key], value);
      if (data) return data;
    }
    return null;
  }

  async set(key: string, value: string, data: any) {
    if (typeof data == 'string')
      data = {
        key: value,
        value: data,
      };
    let rows = await this.knex.select('*').from(this.table).where(key, value);
    if (!rows.length) {
      await this.knex(this.table).insert(data);
    } else {
      await this.knex(this.table).where(key, value).update(data);
    }
  }

  async setJSON(key: string, data: object) {
    this.set('key', key, JSON.stringify(data));
  }

  async delete(key: string, value: string) {
    await this.knex(this.table).where(key, value).del();
  }

  async ensure(key: string, value: string, data: Object) {
    if (typeof data == 'string')
      data = {
        key: value,
        value: data,
      };
    let rows = await this.knex.select('*').from(this.table).where(key, value);
    if (!rows.length) {
      await this.knex(this.table).insert(data);
    }
  }

  async ensureJSON(key: string, data: Object) {
    await this.ensure('key', key, JSON.stringify(data));
  }
}

var keyValueSchema = (t: Knex.TableBuilder) => {
  t.string('key').primary();
  t.json('value');
};

var playerSchema = (t: Knex.TableBuilder) => {
  t.string('ID').primary();
  t.string('Hwid');
  t.string('Name');
  t.string('IP');
  t.string('Icon');
  t.string('GroupID');
  t.string('LastServer');
  t.bigInteger('FirstJoin');
  t.bigInteger('LastJoin');
  t.bigInteger('PlayTime');
  t.integer('Kills');
  t.integer('Deaths');
  t.integer('Experience');
  t.integer('Reputation');
  t.boolean('Banned');
  t.boolean('VpnOrProxy');
  t.string('PrivacyState');
  t.boolean('VacBanned');
  t.boolean('LimitedAccount');
};

var linksSchema = (t: Knex.TableBuilder) => {
  t.string('Steam').primary();
  t.string('Code');
  t.string('Discord');
  t.integer('Votes');
};
