import {
	Component as _Component,
	useState as _useState,
	Dispatch,
	SetStateAction,
} from 'react';

// Globally available modules must be declared here
// Copy type definitions from @types/react/index.d.ts, because namespaces can't be re-exported

declare global {
	namespace React {
		function useState<S>(
			initialState: S | (() => S),
		): [S, Dispatch<SetStateAction<S>>];
		function useState<S = undefined>(): [
			S | undefined,
			Dispatch<SetStateAction<S | undefined>>
		];
		interface Component<P = {}, S = {}, SS = any>
			extends ComponentLifecycle<P, S, SS> {}
	}
	const Reactstrap: any;
	const PropTypes: any;
}

export const test = 1;
