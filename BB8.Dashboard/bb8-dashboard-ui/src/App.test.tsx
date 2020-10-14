import React from 'react';
import { render } from '@testing-library/react';
import App from './App';

test('renders gpio capabilities header', () => {
  const { getByText } = render(<App />);
  const tableHEaderElement = getByText(/capabilities/i);
  expect(tableHEaderElement).toBeInTheDocument();
});
