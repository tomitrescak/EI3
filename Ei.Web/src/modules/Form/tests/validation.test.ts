import sinon from 'sinon';

import {
  email, eq, gt, gte, int, is, isEmail, isInt, isNumber, isRequired, link, lt, lte, maxLength,
  minLength, number, regEx, required, sameAs
} from '../library/validation';

describe('Validation', () => {
  const fakeContext: any = {};
  it('tests isRequired', () => {
    expect(isRequired(undefined, fakeContext)).toBe('This field is required!');
    expect(isRequired('', fakeContext)).toBe('This field is required!');
    expect(isRequired('vale', fakeContext)).toBeNull();
  });

  it('tests required with message', () => {
    expect(required('Required!')(undefined, fakeContext)).toBe('Required!');
    expect(required('Required!')('value', fakeContext)).toBeNull();
  });

  it('tests isInt', () => {
    expect(isInt(1.34, fakeContext)).toBe('Expected integer');
    expect(isInt('1.34', fakeContext)).toBe('Expected integer');

    expect(isInt(undefined, fakeContext)).toBeNull();
    expect(isInt('1', fakeContext)).toBeNull();
    expect(isInt(2, fakeContext)).toBeNull();
    expect(isInt(-4, fakeContext)).toBeNull();
  });

  it('tests integer with message', () => {
    expect(int('Integer!')('a', fakeContext)).toBe('Integer!');

    expect(int('Integer!')(undefined, fakeContext)).toBeNull();
    expect(int('Integer!')('', fakeContext)).toBeNull();
    expect(int('Integer!')(null, fakeContext)).toBeNull();
    expect(int('Integer!')(0, fakeContext)).toBeNull();
  });

  it('tests number', () => {
    expect(isNumber('aaa')).toBe('Expected number');

    expect(isNumber('')).toBeNull();
    expect(isNumber(null)).toBeNull();
    expect(isNumber(undefined)).toBeNull();
    expect(isNumber('1.34')).toBeNull();
    expect(isNumber(1.34)).toBeNull();
    expect(isNumber('1')).toBeNull();
    expect(isNumber(2)).toBeNull();
    expect(isNumber(-4)).toBeNull();
  });

  it('tests number with message', () => {
    expect(number('Number!')(0, fakeContext)).toBeNull();
    expect(number('Number!')(undefined, fakeContext)).toBeNull();
    expect(number('Number!')('', fakeContext)).toBeNull();
    expect(number('Number!')('a', fakeContext)).toBe('Number!');
  });

  it('tests minimum value', () => {
    expect(lt()('rr', fakeContext)).toBe('Expected number');
    expect(lte()('rr', fakeContext)).toBe('Expected number');

    expect(lt(5)(5, fakeContext)).toBe('Must be < 5');
    expect(lte(5)(6, fakeContext)).toBe('Must be <= 5');

    expect(lt(5)(5, fakeContext)).toBe('Must be < 5');
    expect(lte(5)(5, fakeContext)).toBeNull();

    expect(lt(3)(2, fakeContext)).toBeNull();

    expect(lte(3)(3, fakeContext)).toBeNull();
    expect(lte(5)(2, fakeContext)).toBeNull();

    // with message

    expect(lt(3, 'Error')(4, fakeContext)).toBe('Error');
    expect(lte(3, 'Error')(5, fakeContext)).toBe('Error');
  });

  it('tests maximum value', () => {
    expect(gt()('rr', fakeContext)).toBe('Expected number');
    expect(gte()('rr', fakeContext)).toBe('Expected number');

    expect(gt(5)(5, fakeContext)).toBe('Must be > 5');
    expect(gte(5)(5, fakeContext)).toBeNull();

    expect(gt(3)(2, fakeContext)).toBe('Must be > 3');
    expect(gt(5)(5, fakeContext)).toBe('Must be > 5');

    expect(gte(3)(2, fakeContext)).toBe('Must be >= 3');

    expect(gt(5)(6, fakeContext)).toBeNull();
    expect(gte(5)(5, fakeContext)).toBeNull();

    // with message

    expect(gt(3, 'Error')(2, fakeContext)).toBe('Error');
    expect(gte(3, 'Error')(2, fakeContext)).toBe('Error');
  });

  it('tests email', () => {
    expect(isEmail('eee', fakeContext)).toBe('Expected email');
    expect(isEmail('eee@eee', fakeContext)).toBe('Expected email');
    expect(isEmail('eee@eee.ee', fakeContext)).toBeNull();

    // with message
    expect(email('Email!')('eee', fakeContext)).toBe('Email!');
    expect(email('Email!')('eee@eee.ee', fakeContext)).toBeNull();
  });

  it('tests minLength', () => {
    expect(minLength(5)(1234, fakeContext)).toBe('Minimum 5 characters');
    expect(minLength(5)('123', fakeContext)).toBe('Minimum 5 characters');
    expect(minLength(5, 'Five')('eee', fakeContext)).toBe('Five');
    expect(minLength(5)('123456', fakeContext)).toBeNull();
  });

  it('tests maxLength', () => {
    expect(maxLength(5)(123456, fakeContext)).toBe('Maximum 5 characters');
    expect(maxLength(5)('1234567', fakeContext)).toBe('Maximum 5 characters');
    expect(maxLength(5, 'Five')('1234567', fakeContext)).toBe('Five');
    expect(maxLength(5)('1234', fakeContext)).toBeNull();
  });

  it('tests equality', () => {
    expect(eq(5)(6, fakeContext)).toBe('Value 6 does not match 5');

    expect(eq(5)(5, fakeContext)).toBeNull();
    expect(eq(5)('5', fakeContext)).toBeNull();
    expect(eq('5')(5, fakeContext)).toBeNull();
    expect(eq(null)(null, fakeContext)).toBeNull();

    expect(eq(() => 5)(5, fakeContext)).toBeNull();
    expect(eq(() => 6)(5, fakeContext)).toBe('Value 5 does not match 6');

    expect(eq(5)(null, fakeContext)).toBe('Value null does not match 5');
    expect(eq(5, 'Neq')(null, fakeContext)).toBe('Neq');
  });

  it('tests hard equality', () => {
    expect(is(() => 5)(5, fakeContext)).toBeNull();
    expect(is(5)(5, fakeContext)).toBeNull();

    expect(is(() => '5')(5, fakeContext)).toBe('Value 5 does not match "5"');
    expect(is(5)('5', fakeContext)).toBe('Value "5" does not match 5');
    expect(is(5, 'Neq')(null, fakeContext)).toBe('Neq');
  });

  it('tests regexp', () => {
    expect(regEx(/\d/)('a', fakeContext)).toBe('Unexpected format');
    expect(regEx(/\d/, 'Reg')('a', fakeContext)).toBe('Reg');

    expect(regEx(/\d/)(2, fakeContext)).toBeNull();
  });

  it('tests to be same as another field', () => {
    expect(sameAs('pass2')('p', { document: { pass2: 'p' } } as any)).toBeNull();
    expect(sameAs('pass2')('p', { document: { pass2: 'l' } } as any)).toBe(
      'Value must match with "pass2"'
    );
    expect(
      sameAs('pass2', 'Passwords do not match')('p', { document: { pass2: 'l' } } as any)
    ).toBe('Passwords do not match');
  });

  it('executes linked test', () => {
    const spy = sinon.spy();

    // execute link and make sure the linked validation was executed
    link('name')(null, {
      document: { name: 'p' },
      validators: {
        name: [spy]
      }
    } as any);

    expect(spy).toHaveBeenCalled();

    // when value is null the linked validation does not execute
    spy.resetHistory();

    link('name')(null, {
      document: { name: null },
      validators: {
        name: [spy]
      }
    } as any);

    expect(spy).not.toHaveBeenCalled();
  });
});
