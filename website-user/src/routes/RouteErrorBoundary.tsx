import { Component } from 'react';
import type React from 'react';
import { ErrorBox } from '../components/common/ErrorBox';

export class RouteErrorBoundary extends Component<
  { fallbackText: string; children: React.ReactNode },
  { hasError: boolean }
> {
  state = { hasError: false };

  static getDerivedStateFromError() {
    return { hasError: true };
  }

  componentDidCatch(error: Error) {
    console.error(error);
  }

  render() {
    if (this.state.hasError) {
      return (
        <section className="shell py-12">
          <ErrorBox text={this.props.fallbackText} />
        </section>
      );
    }

    return this.props.children;
  }
}
